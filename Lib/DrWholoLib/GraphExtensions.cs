using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft_Graph_UWP_Connect_SDK
{
    static public class GraphExtensions
    {
        static public async Task<T> GetAsAsync<T>(this IDirectoryObjectWithReferenceRequest request) where T:DirectoryObject
        {
            return (await request.GetAsync()) as T;
        }
    }
}
