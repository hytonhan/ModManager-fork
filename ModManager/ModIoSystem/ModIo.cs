using Modio;

namespace ModManager.ModIoSystem
{
    public class ModIo
    {
        private static ModIo? _instance;

        public ModIo()
        {
            Client = new Client(new Credentials("7f52d134de5cde63fdcf163478e688e3"));
        }

        public static ModIo Instance => _instance ??= new ModIo();

        public readonly Client Client;
    }
}