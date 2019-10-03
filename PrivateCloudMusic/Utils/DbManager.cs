using LiteDB;

namespace Pcm.Utils
{
    public class DbManager
    {
        public readonly static LiteDatabase DB;

        static DbManager()
        {
            var dbfile = ConfigManager.Get("dbFile", "db.litedb");
            
            DB = new LiteDatabase(dbfile);
        }
    }
}