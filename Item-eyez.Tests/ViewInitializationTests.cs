using System;
using System.Threading;
using Item_eyez.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Item_eyez.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class ViewInitializationTests
    {
        private static Exception? RunOnSta(Func<object> ctor)
        {
            var mre = new ManualResetEvent(false);
            Exception? captured = null;

            var thread = new Thread(() =>
            {
                try
                {
                    var _ = ctor();
                }
                catch (Exception ex)
                {
                    captured = ex;
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
            thread.Join();
            return captured;
        }

        [TestMethod]
        public void Initialize_ContainersView()
        {
            var ex = RunOnSta(() => new Containers_view());
            if (ex != null)
            {
                Assert.Fail($"View initialization failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void Initialize_ItemsView()
        {
            var ex = RunOnSta(() => new Items_view());
            if (ex != null)
            {
                Assert.Fail($"View initialization failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void Initialize_MainView()
        {
            var ex = RunOnSta(() => new Main_view());
            if (ex != null)
            {
                Assert.Fail($"View initialization failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void Initialize_OrganizeView()
        {
            var ex = RunOnSta(() => new Organize_view());
            if (ex != null)
            {
                Assert.Fail($"View initialization failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void Initialize_RoomsView()
        {
            var ex = RunOnSta(() => new Rooms_view());
            if (ex != null)
            {
                Assert.Fail($"View initialization failed: {ex.Message}");
            }
        }
    }
}
