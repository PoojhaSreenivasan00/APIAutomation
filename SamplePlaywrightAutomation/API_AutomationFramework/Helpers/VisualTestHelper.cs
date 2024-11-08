using AventStack.ExtentReports;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace API_AutomationFramework.Helpers
{
    public static class VisualTestHelper
    {
        public static IPage? Page;
        public static string VisualTestOutputDirectory = "";
        private static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[5][]
        {
              new float[5]{ 0.3f, 0.3f, 0.3f, 0.0f, 0.0f },
              new float[5]{ 0.59f, 0.59f, 0.59f, 0.0f, 0.0f },
              new float[5]{ 0.11f, 0.11f, 0.11f, 0.0f, 0.0f },
              new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
              new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
                });


        public static ComparisonResult Differences(
          Image img1,
          Image img2,
          ComparisonOptions? options = null)
        {
            if (options == null)
                options = new ComparisonOptions();
            byte[,] differenceMatrix = GetDifferenceMatrix(img1, img2, options.Threshold);
            int num1 = CountDifferingPixels(differenceMatrix);
            float num2 = num1 / (float)differenceMatrix.Length;
            ComparisonResult comparisonResult = new ComparisonResult()
            {
                Match = num1 == 0,
                DifferencePercentage = num2
            };
            if (!comparisonResult.Match && options.CreateDifferenceImage)
            {
                CreateDifferenceImage(img1, img2, options);
                comparisonResult.DifferenceImage = GetDifferenceImage(img1, differenceMatrix, options.ShowCellValues);
            }
            return comparisonResult;
        }

        private static int CountDifferingPixels(byte[,] differences)
        {
            int num = 0;
            byte[,] numArray = differences;
            int upperBound1 = numArray.GetUpperBound(0);
            int upperBound2 = numArray.GetUpperBound(1);
            for (int lowerBound1 = numArray.GetLowerBound(0); lowerBound1 <= upperBound1; ++lowerBound1)
            {
                for (int lowerBound2 = numArray.GetLowerBound(1); lowerBound2 <= upperBound2; ++lowerBound2)
                {
                    if (numArray[lowerBound1, lowerBound2] > 0)
                        ++num;
                }
            }
            return num;
        }

        private static void CreateDifferenceImage(Image img1, Image img2, ComparisonOptions options)
        {
            byte[,] differenceMatrix = GetDifferenceMatrix(img1, img2, options.Threshold);
            using (Bitmap differenceImage = GetDifferenceImage(img2, differenceMatrix, options.ShowCellValues))
                differenceImage.Save(VisualTestOutputDirectory + Guid.NewGuid() + ".png");
        }

        private static Bitmap GetDifferenceImage(
          Image baseImage,
          byte[,] differences,
          bool showCellValues)
        {
            Bitmap bitmap = new Bitmap(baseImage);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int index1 = 0; index1 < differences.GetLength(1); ++index1)
                {
                    for (int index2 = 0; index2 < differences.GetLength(0); ++index2)
                    {
                        byte difference = differences[index2, index1];
                        if (difference > 0)
                        {
                            Font font = new Font("Arial", 8f);
                            string str = difference.ToString();
                            SizeF sizeF = graphics.MeasureString(str, font);
                            Rectangle rectangle = new Rectangle(index2 * 16, index1 * 16, 16, 16);
                            graphics.DrawRectangle(Pens.DarkMagenta, rectangle);
                            graphics.FillRectangle(new SolidBrush(Color.FromArgb(64, 139, 0, 139)), rectangle);
                            if (showCellValues)
                            {
                                graphics.DrawString(str, font, Brushes.Black, (float)(index2 * 16 + 8 - (double)sizeF.Width / 2.0 + 1.0), (float)(index1 * 16 + 8 - (double)sizeF.Height / 2.0 + 1.0));
                                graphics.DrawString(str, font, Brushes.White, index2 * 16 + 8 - sizeF.Width / 2f, index1 * 16 + 8 - sizeF.Height / 2f);
                            }
                        }
                    }
                }
            }
            return bitmap;
        }

        private static byte[,] GetDifferenceMatrix(Image img1, Image img2, byte threshold)
        {
            int targetWidth = img1.Width / 16;
            int targetHeight = img1.Height / 16;
            byte[,] numArray = new byte[targetWidth, targetHeight];
            using (Bitmap bitmap1 = (Bitmap)img1.PrepareForComparison(targetWidth, targetHeight))
            {
                using (Bitmap bitmap2 = (Bitmap)img2.PrepareForComparison(targetWidth, targetHeight))
                {
                    for (int index1 = 0; index1 < targetHeight; ++index1)
                    {
                        for (int index2 = 0; index2 < targetWidth; ++index2)
                        {
                            Color pixel = bitmap1.GetPixel(index2, index1);
                            byte r1 = pixel.R;
                            pixel = bitmap2.GetPixel(index2, index1);
                            byte r2 = pixel.R;
                            byte num = (byte)Math.Abs(r1 - r2);
                            if (num < threshold)
                                num = 0;
                            numArray[index2, index1] = num;
                        }
                    }
                }
            }
            return numArray;
        }

        private static Image PrepareForComparison(
          this Image original,
          int targetWidth,
          int targetHeight)
        {
            Image image = new Bitmap(targetWidth, targetHeight);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(ColorMatrix);
                imageAttributes.SetWrapMode((WrapMode)3);
                Rectangle rectangle = new Rectangle(0, 0, targetWidth, targetHeight);
                graphics.DrawImage(original, rectangle, 0, 0, original.Width, original.Height, (GraphicsUnit)2, imageAttributes);
            }
            return image;
        }

        public static bool VisualValidate(IPage Page, Bitmap baseImage, Bitmap verifyImage, string message = "", byte threshold = 20)
        {
            ComparisonOptions options = new ComparisonOptions()
            {
                CreateDifferenceImage = true,
                Threshold = threshold
            };

            ComparisonResult comparisonResult = Differences(baseImage, verifyImage, options);

            Debug.WriteLine("Match: " + comparisonResult.Match.ToString());
            Debug.WriteLine("Match: " + comparisonResult.DifferencePercentage.ToString());

            bool result = comparisonResult.Match || comparisonResult.DifferencePercentage * 100 < 5;

            TestReport.LogScreenshot((result ? "Passed" : "Failed") + " Visual Test! " + message, Page, comparisonResult.DifferenceImage != null ? comparisonResult.DifferenceImage : verifyImage, result ? Status.Pass : Status.Fail);

            return result;
        }

        public static Bitmap ByteArrayToBitmap(byte[] byteArrayIn)
        {

            MemoryStream ms = new MemoryStream(byteArrayIn);
            Bitmap bm = new Bitmap(ms);
            return bm;
        }


    }
}
