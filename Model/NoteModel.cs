using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;


namespace NoteApp.Model
{
    public class NoteModel
    {
        private readonly string connectionString;

        // Конструктор: задаёт путь к базе данных и выполняет инициализацию
        public NoteModel(string databasePath)
        {
            connectionString = $"Data Source={databasePath};";
            initializeDatabase(databasePath);
        }

        // Метод инициализации базы данных и таблиц
        private void initializeDatabase(string databasePath)
        {
            // Создаём таблицы
            createNotesTable(); // Таблица заметок
            createFilesTable(); // Таблица файлов (Важно, тут храняться пути до файлов. Не более.)
            createMetaTable();  // Таблица мета-данных (может пригодиться, если мы хотим чтобы файл отображался где-то конкретно, примениельно к какому-то по счету знаку и тп)
        }

        // Создать таблицу notes
        public void createNotesTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                CREATE TABLE IF NOT EXISTS notes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title TEXT NOT NULL,
                    content TEXT NOT NULL,
                    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
                var command = new SqliteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        public string[] getCategories()
        {
            string[] categorizedNotes = { "Дом", "Работа", "Учеба" };

            return categorizedNotes;
        }

        // Создать таблицу noteFiles
        public void createFilesTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                CREATE TABLE IF NOT EXISTS noteFiles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    noteId INTEGER NOT NULL,
                    filePath TEXT NOT NULL,
                    FOREIGN KEY (noteId) REFERENCES notes(id) ON DELETE CASCADE
                )";
                var command = new SqliteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        // Создать таблицу noteMetaSettings
        public void createMetaTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                CREATE TABLE IF NOT EXISTS noteMetaSettings (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    noteId INTEGER NOT NULL,
                    key TEXT NOT NULL,
                    value TEXT NOT NULL,
                    FOREIGN KEY (noteId) REFERENCES notes(id) ON DELETE CASCADE
                )";
                var command = new SqliteCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        // Добавить новую заметку
        public void addNote(string title, string content)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO notes (title, content) VALUES (@title, @content)";
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@content", content);
                command.ExecuteNonQuery();
            }
        }

        // Добавить файл к заметке
        public void addFileToNote(int noteId, string filePath)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO noteFiles (noteId, filePath) VALUES (@noteId, @filePath)";
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@noteId", noteId);
                command.Parameters.AddWithValue("@filePath", filePath);
                command.ExecuteNonQuery();
            }
        }

        // Добавить мета-данное к заметке
        public void addMetaToNote(int noteId, string key, string value)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO noteMetaSettings (noteId, key, value) VALUES (@noteId, @key, @value)";
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@noteId", noteId);
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@value", value);
                command.ExecuteNonQuery();
            }
        }

        // Получить все заметки с файлами и мета-данными
        public List<Note> getAllNotesWithFilesAndMeta()
        {
            var notes = new List<Note>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string queryNotes = "SELECT id, title, content, createdAt FROM notes";
                var commandNotes = new SqliteCommand(queryNotes, connection);

                using (var readerNotes = commandNotes.ExecuteReader())
                {
                    while (readerNotes.Read())
                    {
                        var note = new Note
                        {
                            id = readerNotes.GetInt32(0),
                            title = readerNotes.GetString(1),
                            content = readerNotes.GetString(2),
                            createdAt = readerNotes.GetDateTime(3),
                            files = new List<string>(),
                            metaData = new Dictionary<string, string>()
                        };

                        // Получить файлы для заметки
                        string queryFiles = "SELECT filePath FROM noteFiles WHERE noteId = @noteId";
                        var commandFiles = new SqliteCommand(queryFiles, connection);
                        commandFiles.Parameters.AddWithValue("@noteId", note.id);

                        using (var readerFiles = commandFiles.ExecuteReader())
                        {
                            while (readerFiles.Read())
                            {
                                note.files.Add(readerFiles.GetString(0));
                            }
                        }

                        // Получить мета-данные для заметки
                        string queryMeta = "SELECT key, value FROM noteMetaSettings WHERE noteId = @noteId";
                        var commandMeta = new SqliteCommand(queryMeta, connection);
                        commandMeta.Parameters.AddWithValue("@noteId", note.id);

                        using (var readerMeta = commandMeta.ExecuteReader())
                        {
                            while (readerMeta.Read())
                            {
                                note.metaData[readerMeta.GetString(0)] = readerMeta.GetString(1);
                            }
                        }

                        notes.Add(note);
                    }
                }
            }
            return notes;
        }
    }

    // Класс модели Note
    public class Note
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime createdAt { get; set; }
        public List<string> files { get; set; } = new List<string>();
        public Dictionary<string, string> metaData { get; set; } = new Dictionary<string, string>();
    }

}
