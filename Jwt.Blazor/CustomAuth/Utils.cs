using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Text;

namespace Agy.Blazor.Classes
{
    public static class Utils
    {
        public static string ShortenJWT(string token)
        {
            // Pass back the first 3 & last 10 digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 3)}...{token.Substring(token.Length - 10, 10)}";
            return "no token!";
        }

        public static IEnumerable<Claim> ValidateJwt(string token, string secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var name = jwtToken.Claims.FirstOrDefault(o => o?.Type == "name")?.Value;

                Debug.WriteLine("    Token  VALIDATED");
               // Debug.WriteLine($"    Jwt data: name = {name}  :  user_type = {ut}");
                Debug.WriteLine($"    Jwt data: name = {name}");

                return jwtToken.Claims;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes

                Debug.WriteLine("    Token  NOT  VALID");
                return null;
            }
        }


        static String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }


        public static byte[] StreamToBytes(Stream input)
        {
            // SO question here: https://stackoverflow.com/a/33611922/425357
            if (input is MemoryStream)
                return ((MemoryStream)input).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        #region Old Stream to Byte[] Methods

        // These dont work proporly, they return 0 bytes - thisnk its got something to do with the using{...} in SixImages

        //public static byte[] StreamToBytes(Stream input)
        //{
        //    // SO question here: https://stackoverflow.com/a/221941/425357
        //    byte[] buffer = new byte[16 * 1024];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        //public static byte[] StreamToBytes(Stream input)
        //{
        //    // SO question here: https://stackoverflow.com/a/7073124/425357
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        input.CopyTo(memoryStream);
        //        var bytes = memoryStream.ToArray();
        //        return bytes;
        //    }
        //}

        #endregion







        //public ImageSource ConvertBytesToImageSource(byte[] ImageBytes)
        //{
        //   // byte[] imageData = (byte[])value;

        //    BitmapImage biImg = new BitmapImage();
        //    MemoryStream ms = new MemoryStream(ImageBytes);
        //    biImg.BeginInit();
        //    biImg.StreamSource = ms;
        //    biImg.EndInit();

        //    ImageSource imgSrc = biImg as ImageSource;

        //    return imgSrc;
        //}

        //public byte[] GetImageAsByteArray(string imageName)
        //{
        //    //var streamResourceInfo = Application.GetResourceStream(new Uri("YourProjectNameSpace;component/ImageFolderName/" + imageName), UriKind.Relative);
        //    var streamResourceInfo = Application.GetResourceStream(new Uri(imageName));
        //    byte[] image = { };
        //    if (streamResourceInfo != null)
        //    {
        //        var length = streamResourceInfo.Stream.Length;
        //        image = new byte[length];
        //        streamResourceInfo.Stream.Read(image, 0, (int)length);
        //    }
        //    return image;
        //}


        //public BitmapImage ByteToBitmapImage(byte data)
        //{
        //    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
        //    {
        //        var imageSource = new BitmapImage();
        //        imageSource.BeginInit();
        //        imageSource.StreamSource = ms;
        //        imageSource.CacheOption = BitmapCacheOption.OnLoad;
        //        imageSource.EndInit();

        //        // Assign the Source property of your image
        //        //yourImage.Source = imageSource;
        //        return imageSource;
        //    }
        //}

        //public byte[] GetJPGFromImageControl(BitmapImage imageC)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(imageC));
        //    encoder.Save(memStream);
        //    return memStream.ToArray();
        //}

        //public static byte[] ImageToByteArray(Image imageIn)
        //{
        //    byte[] data;
        //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(imageIn));
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        encoder.Save(ms);
        //        data = ms.ToArray();
        //    }
        //    return data;
        //}

        //public byte[] ImageToByteArray(Image imageIn)
        //{
        //    MemoryStream ms = new MemoryStream();

        //    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        //    return ms.ToArray();
        //}
    }
}
