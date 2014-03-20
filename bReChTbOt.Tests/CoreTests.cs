using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bReChTbOt;
using bReChTbOt.Core;
using bReChTbOt.Map;
using System.Collections.Generic;

namespace bReChTbOt.Tests
{
	[TestClass]
	public class CoreTests
	{
		[TestMethod]
		public void TestMethod1()
		{
            //Arrange
            Region region = new Region() { ID = 1 };
            SuperRegion superregion = new SuperRegion() { ID = 1 };

            //Act
            superregion.AddChildRegion(region);
            int childcount = superregion.ChildRegions.Count;

            //Assert
            Assert.IsTrue(childcount == 1);
		}
	}
}
