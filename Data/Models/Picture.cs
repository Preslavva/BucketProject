namespace BucketProject.Data.Models
{
    public class Image
    {
        public int Id { get; set; }
        public byte[] VBImage { get; set; }

        public bool IsDarkened { get; set; }

        public bool IsDeleted { get; set; }


        public Image(int id, byte[] image, bool isDarkened, bool isDeleted)
        {
            this.Id = id;
            this.VBImage = image;
            this.IsDarkened = isDarkened;
            this.IsDeleted = isDeleted;
        }

        public Image(byte[] image, bool isDarkened, bool isDeleted)
        {
            this.VBImage = image;
            this.IsDarkened = false;
            this.IsDeleted = false;
        }

        public Image(){}
    }
}
