using System.Drawing.Drawing2D;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Guardians.Models
{
    public class Utility
    {
        #region 產生密碼鹽

        public const int DefaultSaltSize = 5;

        /// <summary>
        /// 產生Salt
        /// </summary>
        /// <returns>Salt</returns>
        public static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[DefaultSaltSize];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        #endregion

        #region 密碼加密

        /// <summary>
        /// Computes a salted hash of the password and salt provided and returns as a base64 encoded string.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to use in the hash.</param>
        public static string GenerateHashWithSalt(string password, string salt)
        {
            // merge password and salt together
            string sHashWithSalt = password + salt;
            // convert this merged value to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
            // use hash algorithm to compute the hash
            HashAlgorithm algorithm = new SHA256Managed();
            // convert merged bytes to a hash as byte array
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // return the has as a base 64 encoded string
            return Convert.ToBase64String(hash);
        }

        #endregion

        #region 檢查檔案類型是否為pdf

        /// <summary>
        /// 檢查檔案類型是否為pdf
        /// </summary>
        /// <param name="file">檔案</param>
        /// <returns></returns>
        public static bool CheckFileTypeIsPdf(IFormFile file)
        {
            if (file.ContentType.IndexOf("pdf", System.StringComparison.Ordinal) == -1)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 檢查圖片檔案類型

        /// <summary>
        /// 檢查圖片檔案類型
        /// </summary>
        /// <param name="uploadImage">IFormFile 上傳圖檔</param>
        /// <returns></returns>
        public static bool CheckImageType(IFormFile uploadImage)
        {
            if (uploadImage.ContentType.IndexOf("image", System.StringComparison.Ordinal) == -1)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 儲存上傳圖片

        /// <summary>
        /// 儲存上傳圖片
        /// </summary>
        /// <param name="uploadImage">IFormFile 物件</param>
        /// <returns>儲存檔名</returns>
        public static string SaveImage(IFormFile uploadImage)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmsss") + Path.GetExtension(uploadImage.FileName);
            string path = $@"wwwroot\Uploads\{fileName}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                uploadImage.CopyTo(stream);
            }

            return fileName;
        }

        #endregion

        #region 指定字串長度

        public static string SetSubjectLength(int limit, string subject)
        {
            if (!string.IsNullOrWhiteSpace(subject))
            {
                if (subject.Length > limit)
                {
                    subject = subject.Substring(0, limit) + "...";
                }
                return subject;
            }
            return "如圖片文字說明";
        }

        #endregion

        #region 儲存上傳檔案(檔名+日期)
        /// <summary>
        /// 儲存上傳檔案(檔名+日期)
        /// </summary>
        /// <param name="uploadFile">IFormFile 物件</param>
        /// <returns>儲存檔名</returns>
        public static string SaveFile(IFormFile uploadFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(uploadFile.FileName) + DateTime.Now.ToString("yyyyMMdd") + Path.GetExtension(uploadFile.FileName).ToLower();
            string path = $@"wwwroot\Uploads\{fileName}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                uploadFile.CopyTo(stream);
            }
            return fileName;

        }
        #endregion

        #region 儲存上傳檔案(原檔名)+檢查檔案是否重複
        /// <summary>
        /// 儲存上傳檔案(原檔名)+檢查檔案是否重複
        /// </summary>
        /// <param name="uploadFile">IFormFile 物件</param>
        /// <returns>儲存檔名</returns>
        public static string SaveOriginFile(IFormFile uploadFile)
        {
            string newPath = Utility.IsFileExist(@"wwwroot\Uploads", uploadFile);
            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                uploadFile.CopyTo(stream);
            }
            return new FileInfo(newPath).Name;

        }
        #endregion

        #region 儲存上傳檔案(原檔名)+檢查檔案是否重複
        /// <summary>
        /// 儲存上傳檔案(原檔名)+檢查檔案是否重複
        /// </summary>
        /// <param name="pathWithoutFileName">無檔案名稱的路徑</param>
        /// <param name="uploadFile">IFormFile 物件</param>
        /// <returns>儲存檔名</returns>
        public static string SaveFileWithFileName(string pathWithoutFileName, IFormFile uploadFile)
        {
            string newPath = Utility.IsFileExist(pathWithoutFileName, uploadFile);
            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                uploadFile.CopyTo(stream);
            }
            return new FileInfo(newPath).Name;

        }
        #endregion

        #region 檢查檔案重複
        /// <summary>
        /// 檢查檔案重複
        /// </summary>
        /// <param name="path">Path without fileName</param>
        /// <param name="uploadFile"></param>
        /// <returns>(string) New Full Path</returns>
        public static string IsFileExist(string path, IFormFile uploadFile)
        {
            int i = 1;
            string extension = Path.GetExtension(uploadFile.FileName).ToLower();
            string temp = $@"{path}\{Path.GetFileNameWithoutExtension(uploadFile.FileName) + extension}";
            string newPath = $@"{path}\{Path.GetFileNameWithoutExtension(uploadFile.FileName)}";
            while (System.IO.File.Exists(temp))
            {
                temp = newPath + $"({i})" + extension;
                i++;
            }

            return temp;
        }

        #endregion

        #region 舉世無敵縮圖程式
        /// <summary>
        /// 舉世無敵縮圖程式(多載)
        /// 1.會自動判斷是比較高還是比較寬，以比較大的那一方決定要縮的尺寸
        /// 2.指定寬度，等比例縮小
        /// 3.指定高度，等比例縮小
        /// </summary>
        /// <param name="name">原檔檔名</param>
        /// <param name="source">來源路徑</param>
        /// <param name="target">目的路徑</param>
        /// <param name="suffix">縮圖辯識符號</param>
        /// <param name="maxWidth">指定要縮的寬度</param>
        /// <param name="maxHight">指定要縮的高度</param>
        /// <remarks></remarks>
        public static void GenerateThumbnailImage(string name, string source, string target, string suffix, int maxWidth, int maxHight)
        {
            Image baseImage = Image.FromFile(source + "\\" + name);
            Single ratio = 0.0F; //存放縮圖比例
            Single h = baseImage.Height;//圖像原尺寸高度
            Single w = baseImage.Width;//圖像原尺寸寬度
            int ht;//圖像縮圖後高度
            int wt; //圖像縮圖後寬度
            if (w > h)
            {//圖像比較寬
                ratio = maxWidth / w;//計算寬度縮圖比例
                if (maxWidth < w)
                {
                    ht = Convert.ToInt32(ratio * h);
                    wt = maxWidth;
                }
                else
                {
                    ht = Convert.ToInt32(baseImage.Height);
                    wt = Convert.ToInt32(baseImage.Width);
                }
            }
            else
            {//比較高
                ratio = maxHight / h;//計算寬度縮圖比例
                if (maxHight < h)
                {
                    ht = maxHight;
                    wt = Convert.ToInt32(ratio * w);
                }
                else
                {
                    ht = Convert.ToInt32(baseImage.Height);
                    wt = Convert.ToInt32(baseImage.Width);
                }
            }
            string newName = target + "\\" + suffix + name;
            Bitmap img = new Bitmap(wt, ht);
            Graphics graphic = Graphics.FromImage(img);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphic.DrawImage(baseImage, 0, 0, wt, ht);
            img.Save(newName);

            img.Dispose();
            graphic.Dispose();
            baseImage.Dispose();

        }

        /// <summary>
        /// 舉世無敵縮圖程式(多載)
        /// 1.會自動判斷是比較高還是比較寬，以比較大的那一方決定要縮的尺寸
        /// 2.指定寬度，等比例縮小
        /// 3.指定高度，等比例縮小
        /// </summary>
        /// <param name="name">原檔檔名</param>
        /// <param name="source">來源檔案的Stream,可接受上傳檔案</param>
        /// <param name="target">目的路徑</param>
        /// <param name="suffix">縮圖辯識符號</param>
        /// <param name="maxWidth">指定要縮的寬度</param>
        /// <param name="maxHight">指定要縮的高度</param>
        /// <remarks></remarks>
        public static void GenerateThumbnailImage(string name, IFormFile uploadImage, string target, string suffix, int maxWidth, int maxHight)
        {
            using (var memoryStream = new MemoryStream())
            {
                uploadImage.CopyTo(memoryStream);
                using (var baseImage = Image.FromStream(memoryStream))
                {

                    Single ratio = 0.0F; //存放縮圖比例
                    Single h = baseImage.Height; //圖像原尺寸高度
                    Single w = baseImage.Width;  //圖像原尺寸寬度
                    int ht; //圖像縮圖後高度
                    int wt;//圖像縮圖後寬度
                    if (w > h)
                    {
                        ratio = maxWidth / w; //計算寬度縮圖比例
                        if (maxWidth < w)
                        {
                            ht = Convert.ToInt32(ratio * h);
                            wt = maxWidth;

                        }
                        else
                        {
                            ht = Convert.ToInt32(baseImage.Height);
                            wt = Convert.ToInt32(baseImage.Width);

                        }
                    }
                    else
                    {
                        ratio = maxHight / h; //計算寬度縮圖比例
                        if (maxHight < h)
                        {
                            ht = maxHight;
                            wt = Convert.ToInt32(ratio * w);
                        }
                        else
                        {
                            ht = Convert.ToInt32(baseImage.Height);
                            wt = Convert.ToInt32(baseImage.Width);
                        }
                    }
                    string newName = target + "\\" + suffix + name;
                    Bitmap img = new Bitmap(wt, ht);
                    Graphics graphic = Graphics.FromImage(img);
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphic.DrawImage(baseImage, 0, 0, wt, ht);
                    img.Save(newName);

                    img.Dispose();
                    graphic.Dispose();
                    baseImage.Dispose();
                }
            }

        }

        /// <summary>
        /// 舉世無敵縮圖程式(指定寬度，等比例縮小)
        /// </summary>
        /// <param name="name">原檔檔名</param>
        /// <param name="source">來源路徑</param>
        /// <param name="target">目的路徑</param>
        /// <param name="suffix">縮圖辯識符號</param>
        /// <param name="maxWidth">指定要縮的寬度</param>
        /// <remarks></remarks>
        public static void GenerateThumbnailImage(int maxWidth, string name, string source, string target, string suffix)
        {
            Image baseImage = Image.FromFile(source + "\\" + name);
            Single ratio = 0.0F; //存放縮圖比例
            Single h = baseImage.Height; //圖像原尺寸高度
            Single w = baseImage.Width; //圖像原尺寸寬度
            int ht; //圖像縮圖後高度
            int wt; //圖像縮圖後寬度
            ratio = maxWidth / w;//計算寬度縮圖比例
            if (maxWidth < w)
            {
                ht = Convert.ToInt32(ratio * h);
                wt = maxWidth;
            }
            else
            {
                ht = Convert.ToInt32(baseImage.Height);
                wt = Convert.ToInt32(baseImage.Width);
            }
            string newName = target + "\\" + suffix + name;

            Bitmap img = new Bitmap(wt, ht);
            Graphics graphic = Graphics.FromImage(img);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphic.DrawImage(baseImage, 0, 0, wt, ht);
            img.Save(newName, System.Drawing.Imaging.ImageFormat.Jpeg);

            img.Dispose();
            graphic.Dispose();
            baseImage.Dispose();
        }

        /// <summary>
        /// 舉世無敵縮圖程式(指定高度，等比例縮小)
        /// </summary>
        /// <param name="name">原檔檔名</param>
        /// <param name="source">來源路徑</param>
        /// <param name="target">目的路徑</param>
        /// <param name="suffix">縮圖辯識符號</param>
        /// <param name="maxHight">指定要縮的高度</param>
        /// <remarks></remarks>
        public static void GenerateThumbnailImage(string name, string source, string target, string suffix, int maxHight)
        {
            Image baseImage = Image.FromFile(source + "\\" + name);
            Single ratio = 0.0F;//存放縮圖比例
            Single h = baseImage.Height; //圖像原尺寸高度
            Single w = baseImage.Width;  //圖像原尺寸寬度
            int ht; //圖像縮圖後高度
            int wt; //圖像縮圖後寬度
            ratio = maxHight / h; //計算寬度縮圖比例
            if (maxHight < h)
            {
                ht = maxHight;
                wt = Convert.ToInt32(ratio * w);

            }
            else
            {
                ht = Convert.ToInt32(baseImage.Height);
                wt = Convert.ToInt32(baseImage.Width);

            }
            string newName = target + "\\" + suffix + name;

            Bitmap img = new Bitmap(wt, ht);
            Graphics graphic = Graphics.FromImage(img);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphic.DrawImage(baseImage, 0, 0, wt, ht);
            img.Save(newName);

            img.Dispose();
            graphic.Dispose();
            baseImage.Dispose();

        }
        #endregion

        #region 取得使用者權限

        /// <summary>
        /// 取得使用者權限
        /// </summary>
        /// <param name="member">使用者</param>
        public static void GetMemberPermission(Member member)
        {
            StringBuilder permissionsBuilder = new StringBuilder();
            foreach (Role role in member.Roles)
            {
                string rp = role.Permission ?? "";
                string[] p = rp.Split(',');
                foreach (string s in p.Where(s => permissionsBuilder.ToString().IndexOf(s + ",") == -1))
                {
                    permissionsBuilder.Append(s + ",");
                }
            }

            string mp = member.Permission ?? "";
            string[] mps = mp.Split(',');
            foreach (string s in mps.Where(s => permissionsBuilder.ToString().IndexOf(s + ",") == -1))
            {
                permissionsBuilder.Append(s + ",");
            }

            member.Permission = permissionsBuilder.ToString();
        }

        #endregion

        #region 取得權限樹

        public static string GetPermissionScript(XmlNode rootnode)
        {
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode node in rootnode.ChildNodes)
            {
                sb.Append(
                    $"{{\"id\": \"{node.Attributes["Value"].Value}\", \"text\": \"{node.Attributes["Title"].Value}\"");
                //要加判斷，不然會無窮遞迴
                if (node.HasChildNodes)
                {
                    sb.Append(",\"children\":[");
                    sb.Append(GetPermissionScript(node));
                    sb.Append("]");
                }

                sb.Append("},");
            }

            string temp = sb.ToString().TrimEnd(',');
            return temp;
        }

        #endregion

        #region 自動將bytes轉換單位
        // Load all suffixes in an array  
        static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
        #endregion

        #region 複製目錄下所有資料夾及檔案

        public static void CopyFiles(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                System.IO.File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        #endregion

    }
}
