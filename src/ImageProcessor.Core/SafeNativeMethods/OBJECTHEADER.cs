using System;
using System.Runtime.InteropServices;

namespace SafeNativeMethods
{
    // for pulling encoded IPictures out of Access Databases
    //
    [StructLayout(LayoutKind.Sequential)]
    public struct OBJECTHEADER
    {
        public short signature; // this looks like it's always 0x1c15
        public short headersize; // how big all this goo ends up being.  after this is the actual object data.
        public short objectType; // we don't care about anything else...they don't seem to be meaningful anyway.
        public short nameLen;
        public short classLen;
        public short nameOffset;
        public short classOffset;
        public short width;
        public short height;
        public IntPtr pInfo;
    }
}