using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace YieldExportReports.Database.DBResources
{
    public static class DBResourceCollectionFactory
    {
        private static readonly string AppDirectory = 
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? @"C:\";

        public static void Serialize(DBResourceCollection dbResourceCollection)
        {
            try
            {
                // 暗号化
                foreach (DBResource dbRes in dbResourceCollection)
                {
                    dbRes.IsEncrypt = true;
                    dbRes.Password = AESCryption.Encrypt(dbRes.Password);
                }

                var xs = new XmlSerializer(typeof(DBResourceCollection));
                using (var writer = new StreamWriter(Path.Combine(AppDirectory, "DBResourceCollection.config")))
                {
                    xs.Serialize(writer, dbResourceCollection);
                }

                //復号化
                foreach (DBResource dbRes in dbResourceCollection)
                {
                    dbRes.Password = AESCryption.Decrypt(dbRes.Password);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static DBResourceCollection DeserializeConfig()
        {
            var filePath = Path.Combine(AppDirectory, "DBResourceCollection.config");
            if (!File.Exists(filePath))
            {
                return new DBResourceCollection();
            }

            var xs = new XmlSerializer(typeof(DBResourceCollection));
            using (var reader = new StreamReader(filePath))
            {
                return xs.Deserialize(reader) as DBResourceCollection ?? new DBResourceCollection();
            }
        }
        public static DBResourceCollection GetDBResource()
        {
            DBResourceCollection dbResourceCollection;
            try
            {
                dbResourceCollection = DeserializeConfig();

                foreach (DBResource dBResource in dbResourceCollection)
                {
                    if (dBResource.IsEncrypt)
                        dBResource.Password = AESCryption.Decrypt(dBResource.Password);
                }
                Serialize(dbResourceCollection);

                return dbResourceCollection;
            }
            catch
            {
                return new DBResourceCollection();
            }
        }
        public static DBResourceCollection GetDBResourceOnLogin()
        {
            DBResourceCollection dbResourceCollection;
            try
            {
                dbResourceCollection = DeserializeConfig();

                foreach (DBResource dBResource in dbResourceCollection)
                {
                    dBResource.IsConnected = false;
                    if (dBResource.IsEncrypt)
                        dBResource.Password = AESCryption.Decrypt(dBResource.Password);
                }
                Serialize(dbResourceCollection);

                return dbResourceCollection;
            }
            catch
            {
                return new DBResourceCollection();
            }
        }
    }
}
