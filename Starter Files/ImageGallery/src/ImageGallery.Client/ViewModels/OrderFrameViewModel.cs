namespace ImageGallery.Client.ViewModels
{
    public class OrderFrameViewModel
    {
        public OrderFrameViewModel(string address)
        {
            this.Address = address;
        }

        public string Address { get; } = string.Empty;
    }
}