using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NoteApp.Service
{
    public class NoteService
    {
        private readonly Model.NoteModel noteDatabase;

        // Конструктор: принимает экземпляр NoteDatabase для записи мета-данных
        public NoteService(Model.NoteModel database)
        {
            noteDatabase = database;
        }

        // Метод для парсинга текста заметки и записи мета-данных
        public void parseAndSaveMeta(int noteId, string content)
        {
            // Регулярное выражение для поиска любых тегов [TAG ...]Текст[/TAG]
            string pattern = @"\[(?<tag>\w+)(?<attributes>[^\]]*?)\](?<innerText>[^\[]+)\[/\k<tag>\]";
            var matches = Regex.Matches(content, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string tag = match.Groups["tag"].Value; // Название тега (например, FILE, IMG)
                    string innerText = match.Groups["innerText"].Value.Trim(); // Текст внутри тега
                    string attributes = match.Groups["attributes"].Value; // Атрибуты тега

                    // Парсим атрибуты
                    var parsedAttributes = parseAttributes(attributes);

                    foreach (var attribute in parsedAttributes)
                    {
                        // Сохраняем каждый атрибут в мета-данные
                        noteDatabase.addMetaToNote(noteId, $"{tag}:{innerText}:{attribute.Key}", attribute.Value);
                    }
                }
            }
        }

        // Метод для извлечения всех тегов из текста (без записи в базу)
        public List<(string tag, string innerText, Dictionary<string, string> attributes)> extractTags(string content)
        {
            var result = new List<(string tag, string innerText, Dictionary<string, string> attributes)>();
            string pattern = @"\[(?<tag>\w+)(?<attributes>[^\]]*?)\](?<innerText>[^\[]+)\[/\k<tag>\]";
            var matches = Regex.Matches(content, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string tag = match.Groups["tag"].Value; // Название тега
                    string innerText = match.Groups["innerText"].Value.Trim(); // Текст внутри тега
                    string attributes = match.Groups["attributes"].Value; // Атрибуты тега

                    // Парсим атрибуты
                    var parsedAttributes = parseAttributes(attributes);

                    result.Add((tag, innerText, parsedAttributes));
                }
            }

            return result;
        }

        // Метод для парсинга атрибутов внутри тега
        private Dictionary<string, string> parseAttributes(string attributes)
        {
            var result = new Dictionary<string, string>();
            string attributePattern = @"(?<key>\w+)\s*=\s*['""](?<value>[^'""]+)['""]";
            var matches = Regex.Matches(attributes, attributePattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string key = match.Groups["key"].Value;
                    string value = match.Groups["value"].Value;
                    result[key] = value;
                }
            }

            return result;
        }
    }
}
