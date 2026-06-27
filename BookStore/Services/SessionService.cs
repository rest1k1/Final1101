using System.IO;

namespace BookStore.Services
{
    /// <summary>
    /// Сервис сохранения текущего пользователя.
    /// </summary>
    public static class SessionService
    {
        private static readonly string SessionFile =
            "session.txt";

        /// <summary>
        /// Сохраняет логин пользователя.
        /// </summary>
        public static void SaveSession(string login)
        {
            File.WriteAllText(SessionFile, login);
        }

        /// <summary>
        /// Загружает логин пользователя.
        /// </summary>
        public static string? LoadSession()
        {
            if (!File.Exists(SessionFile))
            {
                return null;
            }

            return File.ReadAllText(SessionFile);
        }

        /// <summary>
        /// Удаляет сохранённую сессию.
        /// </summary>
        public static void ClearSession()
        {
            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }
        }
    }
}