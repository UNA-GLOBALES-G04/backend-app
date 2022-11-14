using System.Text.RegularExpressions;

namespace webapp.util
{
    public class ArtifactManager
    {
        private IConfiguration configuration;
        private int maxFileSize;
        private string filePath;

        public ArtifactManager(IConfiguration configuration)
        {
            this.configuration = configuration;
            maxFileSize = getMaxFileSize();
            filePath = configuration["Artifact:Path"];
        }


        // saves the multipart file and creates an id for it
        public string SaveFile(IFormFile file)
        {
            // check that the file is not null
            if (file == null)
            {
                throw new Exception("File is null");
            }
            // check that the file is not empty
            if (file.Length == 0)
            {
                throw new Exception("File is empty");
            }
            // check that the file is not too big
            if (file.Length > maxFileSize)
            {
                throw new Exception("File is too big");
            }

            // create a unique id for the file
            var fileId = createId();

            // create the file path
            var path = Path.Combine(filePath, fileId);

            // save the file
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileId;
        }

        private string createId()
        {
            // create a random id, if it already exists, create another one until it is unique
            var id = "";
            var exists = true;
            while (exists)
            {
                id = Guid.NewGuid().ToString();
                exists = checkIfIdExists(id);
            }
            return id;
        }
        private bool checkIfIdExists(string id)
        {
            id.ToLower();
            // check if the id is alphanumeric
            if (!Regex.IsMatch(id, "^[a-zA-Z0-9]*$"))
            {
                throw new Exception("Id is not alphanumeric");
            }
            // check if the id exists in the path
            var path = filePath + id;
            return false;
        }

        private int getMaxFileSize()
        {
            var maxFileSize = configuration["Artifact:MaxFileSize"];
            if (string.IsNullOrEmpty(maxFileSize))
            {
                throw new Exception("Artifact:MaxFileSize is not set in the application configuration");
            }
            // extract the number from the string (e.g. 100MB)
            var maxFileSizeNumber = int.Parse(maxFileSize.Substring(0, maxFileSize.Length - 2));
            // get the unit (e.g. MB)
            var maxFileSizeUnit = maxFileSize.Substring(maxFileSize.Length - 2);
            // power of the unit (e.g. MB = 2^20)
            var unitPower = 0;
            switch (maxFileSizeUnit)
            {
                case "KB":
                    unitPower = 1;
                    break;
                case "MB":
                    unitPower = 2;
                    break;
                case "GB":
                    unitPower = 3;
                    break;
                default:
                    throw new Exception("Artifact:MaxFileSize unit is not supported");
            }
            // calculate the max file size in bytes
            var maxFileSizeInBytes = maxFileSizeNumber * (int)Math.Pow(1024, unitPower);

            return maxFileSizeInBytes;
        }

    }
}