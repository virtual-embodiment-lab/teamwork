using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

[Serializable]
internal class AwsCredentials
{
    public string awsAccessKey;
    public string awsSecretKey;
}

public static class AWSUploader
{
    private static readonly string awsBucketName = "vel-mazeworld";
    private static string awsAccessKey;
    private static string awsSecretKey;
    private static readonly string awsURLBaseVirtual = "https://" + awsBucketName + ".s3.amazonaws.com/";

    static AWSUploader()
    {
        try
        {
            string jsonPath = Path.Combine(Application.dataPath, "..", "AwsKeys.json");
            string jsonContent = File.ReadAllText(jsonPath);
            AwsCredentials credentials = JsonUtility.FromJson<AwsCredentials>(jsonContent);
            awsAccessKey = credentials.awsAccessKey;
            awsSecretKey = credentials.awsSecretKey;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load AWS credentials: {e.Message}");
            awsAccessKey = "";
            awsSecretKey = "";
        }
    }

    public static async Task UploadFileToAWS(string fileName, string filePath)
    {
        try
        {
            Debug.Log("AWS Access Key: " + awsAccessKey);
            Debug.Log("AWS Secret Key: " + awsSecretKey);
            // Automatically create folder path with today's date and hour in UTC
            string folderPath = DateTime.UtcNow.ToString("yyyy-MM-dd-HH");
            string s3Key = $"{folderPath}/{fileName}";

            string currentAWS3Date = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss ") + "GMT";
            string canonicalString = "PUT\n\n\n\nx-amz-date:" + currentAWS3Date + "\n/" + awsBucketName + "/" + s3Key;

            UTF8Encoding encode = new UTF8Encoding();
            using (HMACSHA1 signature = new HMACSHA1())
            {
                signature.Key = encode.GetBytes(awsSecretKey);
                byte[] bytes = encode.GetBytes(canonicalString);
                byte[] moreBytes = signature.ComputeHash(bytes);
                string encodedCanonical = Convert.ToBase64String(moreBytes);

                string aws3Header = "AWS " + awsAccessKey + ":" + encodedCanonical;
                string URL3 = awsURLBaseVirtual + s3Key;

                WebRequest requestS3 = WebRequest.Create(URL3);
                requestS3.Headers.Add("Authorization", aws3Header);
                requestS3.Headers.Add("x-amz-date", currentAWS3Date);
                byte[] fileRawBytes = File.ReadAllBytes(filePath);
                requestS3.ContentLength = fileRawBytes.Length;
                requestS3.Method = "PUT";

                using (Stream S3Stream = await requestS3.GetRequestStreamAsync())
                {
                    await S3Stream.WriteAsync(fileRawBytes, 0, fileRawBytes.Length);
                    Debug.Log($"Sent bytes: {requestS3.ContentLength}, for file: {s3Key}");
                }

                using (WebResponse response = await requestS3.GetResponseAsync())
                {
                    Debug.Log($"Successfully uploaded {s3Key} to S3");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error uploading to S3: {e.Message}");
            throw;
        }
    }
}