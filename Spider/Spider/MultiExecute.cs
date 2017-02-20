using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider
{
    /// <summary>
    /// 重复执行，获取返回结果
    /// </summary>
    public class MultiExecute
    {
        public static TResult Execute<TResult>(Func<TResult> fun, int time)
        {
            for (int i = 0; i < time - 1; i++)
            {
                try
                {
                    return fun();
                }
                catch
                {
                }
            }
            return fun();
        }
        public static TResult Execute<T1, TResult>(Func<T1, TResult> fun, int time, T1 arg1)
        {
            for (int i = 0; i < time - 1; i++)
            {
                try
                {
                    return fun(arg1);
                }
                catch
                {
                }
            }
            return fun(arg1);
        }

        public static TResult Execute<T1, T2, TResult>(Func<T1, T2, TResult> fun, int time, T1 arg1, T2 arg2)
        {
            for (int i = 0; i < time - 1; i++)
            {
                try
                {
                    return fun(arg1, arg2);
                }
                catch
                {
                }
            }
            return fun(arg1, arg2);
        }

        public static TResult Execute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> fun, int time, T1 arg1, T2 arg2, T3 arg3)
        {
            for (int i = 0; i < time - 1; i++)
            {
                try
                {
                    return fun(arg1, arg2, arg3);
                }
                catch
                {
                }
            }
            return fun(arg1, arg2, arg3);
        }
    }
}
