namespace CustomerManagement.Api.Utils
{
    public class PerHttpRequestLifetime : LifetimeManager
    {
        private readonly string _httpContextKey;

        public PerHttpRequestLifetime(string httpContextKey)
        {
            _httpContextKey = httpContextKey;
        }

        public override object GetValue()
        {
            return HttpContext.Current.Items[_httpContextKey];
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[_httpContextKey] = newValue;
        }

        public override void RemoveValue()
        {
            var obj = GetValue();
            HttpContext.Current.Items.Remove(obj);
        }
    }
}