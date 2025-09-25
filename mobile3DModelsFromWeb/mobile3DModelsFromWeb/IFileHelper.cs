namespace mobile3DModelsFromWeb
{
    // Этот интерфейс определяет, ЧТО мы хотим делать, но не говорит, КАК.
    public interface IFileHelper
    {
        /// <summary>
        /// Получает содержимое локального файла в виде строки.
        /// </summary>
        /// <param name="filename">Имя файла (например, "index.html")</param>
        /// <returns>Содержимое файла</returns>
        string GetLocalFileHtml(string filename);

        /// <summary>
        /// Получает базовый URL для локальных файлов.
        /// </summary>
        /// <returns>Строка с URL</returns>
        string GetBaseUrl();
    }
}
