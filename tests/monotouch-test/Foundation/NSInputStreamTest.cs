using System;
using NUnit.Framework;
using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	public class NSInputStreamTest
	{
		[Test]
		public void SubclassedCtor ()
		{
			using (var s = new SubStream ()) {
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		class SubStream : NSInputStream {
		}

		[Test]
		public void Path ()
		{
			using (var s = new NSInputStream ("Info.plist")) {
				// initWithFileAtPath: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

#if !MONOMAC //NSData.FromFile ("Info.plist") returns null.  Trying the same in xcode also returns nil
		[Test]
		public void Data ()
		{
			using (var d = NSData.FromFile ("Info.plist"))
			using (var s = new NSInputStream (d)) {
				// initWithData: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
#endif

		[Test]
		public void Url ()
		{
			using (var u = NSUrl.FromFilename ("Info.plist"))
			using (var s = new NSInputStream (u)) {
				// initWithURL: does not respond (see dontlink.app) but it works
				Assert.That (s.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public unsafe void Read ()
		{
			using (var data = NSData.FromArray (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })) {
				using (var s = NSInputStream.FromData (data)) {
					byte[] arr = new byte[10];
					s.Open ();
					Assert.IsTrue (s.HasBytesAvailable ());
					Assert.AreEqual (2, s.Read (arr, 2), "#a 1");
					Assert.AreEqual (0, arr [0], "#a[0]");
					Assert.AreEqual (1, arr [1], "#a[1]");
				}

				using (var s = new NSInputStream (data)) {
					byte[] arr = new byte[10];
					s.Open ();
					Assert.IsTrue (s.HasBytesAvailable ());
					Assert.AreEqual (2, s.Read (arr, 1, 2), "#b 1");
					Assert.AreEqual (0, arr [0], "#b[0]");
					Assert.AreEqual (0, arr [1], "#b[1]");
					Assert.AreEqual (1, arr [2], "#b[2]");
				}

				using (var s = new NSInputStream (data)) {
					byte[] arr = new byte[10];
					s.Open ();
					Assert.IsTrue (s.HasBytesAvailable ());
					fixed (byte* ptr = &arr[2])
						Assert.AreEqual (2, s.Read ((IntPtr) ptr, 2), "#c 1");
					Assert.AreEqual (0, arr [0], "#c[0]");
					Assert.AreEqual (0, arr [1], "#c[1]");
					Assert.AreEqual (0, arr [2], "#c[2]");
					Assert.AreEqual (1, arr [3], "#c[3]");
				}
			}
		}
	}
}

