using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace SkinsApi.Sources
{
    public class Skin
    {
        private Image _bitmap;
        private bool isSlimSkin = false;
        private bool isTwoLayedSkin = true;

        Point faceStart = new(8, 8);
        Size face = new(8, 8);

        Point bodyStart = new(20, 20);
        Size body = new(9, 12);

        Point rightArmStart = new(44, 20);
        Point leftArmStart = new(36, 52);
        Size slimArm = new(3, 12);
        Size arm = new(4, 12);

        Point leftLegStart = new(20, 52);
        Point rightLegStart = new(4, 29);
        Size leg = new(4, 12);

        Point faceLayoutStart = new(40, 8);
        Point rightArmLayoutStart = new(44, 36);
        Point leftArmLayoutStart = new(52, 52);
        Point bodyLayoutStart = new(20, 36);


        public Skin(Stream skinStream, bool slim = false)
        {
            _bitmap = Image.Load(skinStream);
            isSlimSkin = slim;
            isTwoLayedSkin = _bitmap.Width == 64 && _bitmap.Height == 64;

        }

        public Stream GetAllSkin(float width = 64f)
        {
            var stream = new MemoryStream();
            int resizedWidth = (int)(_bitmap.Width * (width / _bitmap.Width));
            var resizedPicture = _bitmap.Clone(c => c.Resize(resizedWidth, resizedWidth, KnownResamplers.NearestNeighbor));
            resizedPicture.Save(stream, new PngEncoder());
            stream.Position = 0;
            // Сохранение результата
            return stream;
        }

        public Stream GetFace(float width = 8f)
        {
            using (var croppedFace = _bitmap.Clone(k => k.Crop(new(faceStart, face))))
            {
                if (isTwoLayedSkin)
                {
                    var secondLayer = _bitmap.Clone(k => k.Crop(new(faceLayoutStart, face)));
                    croppedFace.Mutate(k => k.DrawImage(secondLayer, new Point(0, 0), 1f));
                }

                var stream = new MemoryStream();
                int resizedWidth = (int)(croppedFace.Width * (width / croppedFace.Width));
                var resizedPicture = croppedFace.Clone(c => c.Resize(resizedWidth, resizedWidth, KnownResamplers.NearestNeighbor));
                resizedPicture.Save(stream, new PngEncoder());
                stream.Position = 0;
                // Сохранение результата
                return stream;
            }
        }

        public Stream GetFull(float width = 128f)
        {

            using (Image finalImage = new Image<Rgba32>(290, 290))
            {
                // Загрузка исходного изображения
                using (Image sourceImage = _bitmap)
                {
                    int armPositionDel = isSlimSkin ? 1 : 0;
                    int armSize = isSlimSkin ? 44 : 58;
                    int armPositionSum = isSlimSkin ? 14 : 0;

                    var slimRightArm = rightArmStart;
                    var slimRightArmLayout = rightArmLayoutStart;
                    slimRightArm.X -= armPositionDel;
                    slimRightArmLayout.X -= armPositionDel;
                    // left arm
                    var leftArm = sourceImage.Clone(ctx => ctx
                        .Crop(new(leftArmStart, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));

                    var leftArmLayout = sourceImage.Clone(c => c
                        .Crop(new(leftArmLayoutStart, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));

                    finalImage.Mutate(ctx => ctx
                        .DrawImage(leftArm, new Point(29 + armPositionSum, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(leftArmLayout, new Point(29 + armPositionSum, 120), 1));

                    // right arm
                    var rightArm = sourceImage.Clone(ctx => ctx
                        .Crop(new(slimRightArm, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));
                    var rightArmLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(slimRightArmLayout, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(rightArm, new Point(200, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(rightArmLayout, new Point(200, 120), 1));

                    // body
                    var bodyImage = sourceImage.Clone(ctx => ctx
                        .Crop(new(bodyStart, body))
                        .Resize(115, 171, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    var bodyImageLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(bodyLayoutStart, body))
                        .Resize((int)(115 * 0.2), (int)(171 * 0.2), KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(bodyImage, new Point(87, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(bodyImageLayout, new Point(87, 120), 1));

                    // head
                    var head = sourceImage.Clone(ctx => ctx
                        .Crop(new(faceStart, face))
                        .Resize(115, 115, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    var headLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(faceLayoutStart, face))
                        .Resize(115, 115, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(head, new Point(87, 5), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(headLayout, new Point(87, 5), 1));
                }

                var stream = new MemoryStream();

                int resizedWidth = (int)(finalImage.Width * (width / finalImage.Width));
                finalImage.Mutate(k => k.Resize(resizedWidth, resizedWidth, KnownResamplers.NearestNeighbor));
                finalImage.Save(stream, new PngEncoder());
                stream.Position = 0;
                return stream;

            }
        }

        public Stream GetBody(float width = 128f)
        {

            using (Image finalImage = new Image<Rgba32>(290, 290))
            {
                // Загрузка исходного изображения
                using (Image sourceImage = _bitmap)
                {
                    int armPositionDel = isSlimSkin ? 1 : 0;
                    int armSize = isSlimSkin ? 44 : 58;
                    int armPositionSum = isSlimSkin ? 14 : 0;

                    var slimRightArm = rightArmStart;
                    var slimRightArmLayout = rightArmLayoutStart;
                    slimRightArm.X -= armPositionDel;
                    slimRightArmLayout.X -= armPositionDel;
                    // left arm
                    var leftArm = sourceImage.Clone(ctx => ctx
                        .Crop(new(leftArmStart, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));

                    var leftArmLayout = sourceImage.Clone(c => c
                        .Crop(new(leftArmLayoutStart, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));

                    finalImage.Mutate(ctx => ctx
                        .DrawImage(leftArm, new Point(29 + armPositionSum, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(leftArmLayout, new Point(29 + armPositionSum, 120), 1));

                    // right arm
                    var rightArm = sourceImage.Clone(ctx => ctx
                        .Crop(new(slimRightArm, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));
                    var rightArmLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(slimRightArmLayout, arm))
                        .Resize(armSize, 171, KnownResamplers.NearestNeighbor));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(rightArm, new Point(200, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(rightArmLayout, new Point(200, 120), 1));

                    // body
                    var bodyImage = sourceImage.Clone(ctx => ctx
                        .Crop(new(bodyStart, body))
                        .Resize(115, 171, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    var bodyImageLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(bodyLayoutStart, body))
                        .Resize(115, 171, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(bodyImage, new Point(87, 120), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(bodyImageLayout, new Point(87, 120), 1));

                    // head
                    var head = sourceImage.Clone(ctx => ctx
                        .Crop(new(faceStart, face))
                        .Resize(115, 115, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    var headLayout = sourceImage.Clone(ctx => ctx
                        .Crop(new(faceLayoutStart, face))
                        .Resize(115, 115, KnownResamplers.NearestNeighbor)); // Изменение размера, если необходимо
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(head, new Point(87, 5), 1));
                    finalImage.Mutate(ctx => ctx
                        .DrawImage(headLayout, new Point(87, 5), 1));
                }

                var stream = new MemoryStream();

                int resizedWidth = (int)(finalImage.Width * (width / finalImage.Width));
                finalImage.Mutate(k => k.Resize(resizedWidth, resizedWidth, KnownResamplers.NearestNeighbor));
                finalImage.Save(stream, new PngEncoder());
                stream.Position = 0;
                return stream;

            }
        }

    }
}
