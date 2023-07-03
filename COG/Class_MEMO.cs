using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COG
{
    class Class_MEMO
    {


//                 Dim blobMeasure As CogBlobMeasure
//         blobMeasure = New CogBlobMeasure
//         blobMeasure.Measure = CogBlobMeasureConstants.Area
//         blobMeasure.Mode = CogBlobMeasureModeConstants.Filter
//         blobMeasure.FilterMode = CogBlobFilterModeConstants.ExcludeBlobsInRange
//         blobMeasure.FilterRangeLow = 0
//         blobMeasure.FilterRangeHigh = 100
//         ' Add to the collection
//         blobTool.RunParams.RunTimeMeasures.Add(blobMeasure)

        // PMBLOBTOOL[0].RunParams.SegmentationParams.SetSegmentationHardFixedThreshold(100,CogBlobSegmentationPolarityConstants.DarkBlobs);     -> 설정시에


//         class SafeMalloc : SafeBuffer
//         {
//             /// <summary>
//             /// Allocates memory and initialises the SaveBuffer
//             /// </summary>
//             /// <param name="size">The number of bytes to allocate</param>
//             public SafeMalloc(int size)
//                 : base(true)
//             {
//                 this.SetHandle(Marshal.AllocHGlobal(size));
//                 this.Initialize((ulong)size);
//             }
// 
//             /// <summary>
//             /// Called when the object is disposed, ferr the
//             /// memory via FreeHGlobal().
//             /// </summary>
//             /// <returns></returns>
//             protected override bool ReleaseHandle()
//             {
//                 Marshal.FreeHGlobal(this.handle);
//                 return true;
//             }
// 
//             /// <summary>
//             /// Cast to IntPtr
//             /// </summary>
//             public static implicit operator IntPtr(SafeMalloc h)
//             {
//                 return h.handle;
//             }
//         }
// 
//         public ICogImage Convert8BitRawImageToCognexImage(
//                 byte[] imageData, int width, int height)
//         {
//             // no padding etc. so size calculation
//             // is simple.
//             var rawSize = width * height;
// 
//             var buf = new SafeMalloc(rawSize);
// 
//             // Copy from the byte array into the
//             // previously allocated. memory
//             Marshal.Copy(imageData, 0, buf, rawSize);
// 
//             // Create Cognex Root thing.
//             var cogRoot = new CogImage8Root();
// 
//             // Initialise the image root, the stride is the
//             // same as the widthas the input image is byte alligned and
//             // has no padding etc.
//             cogRoot.Initialize(width, height, buf, width, buf);
// 
//             // Create cognex 8 bit image.
//             var cogImage = new CogImage8Grey();
// 
//             // And set the image roor
//             cogImage.SetRoot(cogRoot);
// 
//             return cogImage;
//         }

    }
}
