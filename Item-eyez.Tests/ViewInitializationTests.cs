using System.Threading;
using Item_eyez.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Item_eyez.Tests
{
    [TestClass]
    public class ViewInitializationTests
    {
        [TestMethod]
        public void Initialize_ContainersView()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var view = new Containers_view();
                    Assert.IsNotNull(view);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"View initialization failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }

        [TestMethod]
        public void Initialize_ItemsView()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var view = new Items_view();
                    Assert.IsNotNull(view);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"View initialization failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }

        [TestMethod]
        public void Initialize_MainView()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var view = new Main_view();
                    Assert.IsNotNull(view);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"View initialization failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }

        [TestMethod]
        public void Initialize_OrganizeView()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var view = new Organize_view();
                    Assert.IsNotNull(view);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"View initialization failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }

        [TestMethod]
        public void Initialize_RoomsView()
        {
            var mre = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var view = new Rooms_view();
                    Assert.IsNotNull(view);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"View initialization failed: {ex.Message}");
                }
                finally
                {
                    mre.Set();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            mre.WaitOne();
        }
    }
}