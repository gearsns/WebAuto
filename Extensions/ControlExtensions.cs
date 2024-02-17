namespace WebAuto
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Finds the parent for the given control of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="control">control</param>
        /// <returns>Parent or null</returns>
        public static T? GetParentOfType<T>(this Control control) where T : Control
        {
            if (control.IsDisposed || !control.IsHandleCreated)
            {
                return default;
            }
            Control? current = control;
            do
            {
                current = current.Parent;
                if (current == null)
                {
                    return default;
                }
            }
            while (current.GetType() != typeof(T));

            return (T)current;
        }

        public static T? GetParentOfKindType<T>(this Control control) where T : Control
        {
            if (control.IsDisposed || !control.IsHandleCreated)
            {
                return default;
            }
            Control? current = control;
            do
            {
                current = current.Parent;
                if (current == null)
                {
                    return default;
                }
            }
            while (current is not T);

            return (T)current;
        }
    }
}
