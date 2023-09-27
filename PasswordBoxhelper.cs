***REMOVED***


public static class PasswordBoxHelper
***REMOVED***
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty));

    public static string GetPlaceholder(DependencyObject obj)
    ***REMOVED***
        return (string)obj.GetValue(PlaceholderProperty);
***REMOVED***

    public static void SetPlaceholder(DependencyObject obj, string value)
    ***REMOVED***
        obj.SetValue(PlaceholderProperty, value);
***REMOVED***
***REMOVED***
