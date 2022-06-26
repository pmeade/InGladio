namespace lib
{
    public class Meta
    {
        private static Meta[] metas = new Meta[] { new Meta()};
        public static Meta Get(uint index)
        {
            return metas[index % metas.Length];
        }
    }
}