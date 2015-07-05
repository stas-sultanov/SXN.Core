using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of unit tests for <see cref="ArneTree{TKey, TValue}"/> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class ArneTreeTests
	{
		#region Fields

		private readonly Int32[] intInvalidArray = new Int32[100];

		private readonly Int32[] intValidArray = new Int32[100];

		private readonly Random random = new Random(DateTime.Now.Millisecond);

		private ArneTree<Int32, Int32> intArneTree;

		#endregion

		#region Constructor

		[TestInitialize]
		[TestCategory("UnitTests")]
		public void Initialize()
		{
			intArneTree = new ArneTree<Int32, Int32>();

			for (var index = 0; index < intValidArray.Length;)
			{
				var value = random.Next(intValidArray.Length * 2);

				if (intValidArray.Contains(value))
				{
					continue;
				}

				intValidArray[index] = value;

				index++;
			}

			// Add data
			foreach (var t in intValidArray)
			{
				intArneTree.Add(t);
			}

			for (var index = 0; index < intInvalidArray.Length;)
			{
				var value = random.Next(intValidArray.Length * 2, intValidArray.Length * 4);

				if (intInvalidArray.Contains(value))
				{
					continue;
				}

				intInvalidArray[index] = value;

				index++;
			}
		}

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void AddCountContainsTest()
		{
			// Test Count
			Assert.AreEqual(intValidArray.Length, intArneTree.Count);

			// Test Contains
			for (var index = 0; index < intValidArray.Length; index++)
			{
				var item = intValidArray[index];

				var contains = intArneTree.Contains(item);

				Assert.IsTrue(contains);
			}

			// Test Not Contains
			foreach (var contains in intInvalidArray.Select(value => intArneTree.Contains(value)))
			{
				Assert.IsFalse(contains);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ClearContainsTest()
		{
			intArneTree.Clear();

			// Test Count
			Assert.AreEqual(0, intArneTree.Count);

			// Test Contains
			foreach (var contains in intValidArray.Select(value => intArneTree.Contains(value)))
			{
				Assert.IsFalse(contains);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void RemoveContainsTest()
		{
			foreach (var value in intValidArray)
			{
				intArneTree.Remove(value);

				// check contains
				Assert.IsFalse(intArneTree.Contains(value));
			}

			// Test Count
			Assert.AreEqual(0, intArneTree.Count);
		}

		#endregion

		#region NodeBase Tests

		/// <summary>
		/// Tests ctor and following properties: Data, IsLeaf, IsRoot, Level, Left, Right
		/// </summary>
		[TestMethod]
		[TestCategory("UnitTests")]
		public void NodeCtor()
		{
			// test data
			var expectedLeft = new ArneTreeNode<Int32, Int32>(0, 0);
			var expectedRight = new ArneTreeNode<Int32, Int32>(0, 0);

			const UInt32 ExpectedLevel = 777u;
			const Int32 ExpectdData = 100;

			// test object
			var root = new ArneTreeNode<Int32, Int32>(ExpectdData, ExpectedLevel, expectedLeft, expectedRight);

			// tests
			Assert.AreEqual(ExpectedLevel, root.Level);
			Assert.AreEqual(ExpectdData, root.Value);

			// Parent
			Assert.AreEqual(root, expectedLeft.Parent);
			Assert.AreEqual(root, expectedRight.Parent);

			// IsLeaf
			Assert.IsTrue(expectedLeft.IsLeaf);
			Assert.IsFalse(root.IsLeaf);

			// IsRoot
			Assert.IsFalse(expectedLeft.IsRoot);
			Assert.IsTrue(root.IsRoot);

			// left
			Assert.AreEqual(expectedLeft, root.Left);
			root.Left = null;
			Assert.IsNull(expectedLeft.Parent);

			// right
			Assert.AreEqual(expectedRight, root.Right);
			root.Right = null;
			Assert.IsNull(expectedRight.Parent);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NodeSkew()
		{
			// prepare data
			var rootNode = new ArneTreeNode<Char, Char>('0', 0);
			var tNode = new ArneTreeNode<Char, Char>('T', 1);
			var lNode = new ArneTreeNode<Char, Char>('L', 1);
			var aNode = new ArneTreeNode<Char, Char>('A', 2);
			var bNode = new ArneTreeNode<Char, Char>('B', 2);
			var rNode = new ArneTreeNode<Char, Char>('R', 2);

			// initialize
			rootNode.Right = tNode;
			tNode.Left = lNode;
			tNode.Right = rNode;
			lNode.Left = aNode;
			lNode.Right = bNode;

			// perform operation
			Assert.AreEqual(aNode, aNode.Skew());
			Assert.AreEqual(lNode, lNode.Skew());
			rootNode.Right = rootNode.Right.Skew();

			// check results

			// references
			Assert.AreEqual(lNode, rootNode.Right);
			Assert.AreEqual(aNode, lNode.Left);
			Assert.AreEqual(tNode, lNode.Right);
			Assert.AreEqual(bNode, tNode.Left);
			Assert.AreEqual(rNode, tNode.Right);

			// levels
			Assert.AreEqual(0u, rootNode.Level);
			Assert.AreEqual(1u, tNode.Level);
			Assert.AreEqual(1u, lNode.Level);
			Assert.AreEqual(2u, aNode.Level);
			Assert.AreEqual(2u, bNode.Level);
			Assert.AreEqual(2u, rNode.Level);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NodeSplit()
		{
			// prepare data
			var rootNode = new ArneTreeNode<Char, Char>('0', 0);
			var tNode = new ArneTreeNode<Char, Char>('T', 1);
			var rNode = new ArneTreeNode<Char, Char>('R', 1);
			var xNode = new ArneTreeNode<Char, Char>('X', 1);
			var aNode = new ArneTreeNode<Char, Char>('A', 2);
			var bNode = new ArneTreeNode<Char, Char>('B', 2);

			// initialize
			rootNode.Right = tNode;
			tNode.Left = aNode;
			tNode.Right = rNode;
			rNode.Left = bNode;
			rNode.Right = xNode;

			// perform operation
			Assert.AreEqual(xNode, xNode.Split());
			Assert.AreEqual(rNode, rNode.Split());
			Assert.AreEqual(rootNode, rootNode.Split());
			rootNode.Right = rootNode.Right.Split();

			// check results

			// references
			Assert.AreEqual(rNode, rootNode.Right);
			Assert.AreEqual(tNode, rNode.Left);
			Assert.AreEqual(xNode, rNode.Right);
			Assert.AreEqual(aNode, tNode.Left);
			Assert.AreEqual(bNode, tNode.Right);

			// levels
			Assert.AreEqual(0u, rootNode.Level);
			Assert.AreEqual(2u, rNode.Level);
			Assert.AreEqual(1u, tNode.Level);
			Assert.AreEqual(1u, xNode.Level);
			Assert.AreEqual(2u, aNode.Level);
			Assert.AreEqual(2u, bNode.Level);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NodePredecessorAndSuccessor()
		{
			var rootNode = new ArneTreeNode<Int32, Int32>(100, 0);
			var aNode = new ArneTreeNode<Int32, Int32>(30, 1);
			var bNode = new ArneTreeNode<Int32, Int32>(60, 1);
			var cNode = new ArneTreeNode<Int32, Int32>(10, 2);
			var dNode = new ArneTreeNode<Int32, Int32>(20, 2);
			var eNode = new ArneTreeNode<Int32, Int32>(40, 2);
			var fNode = new ArneTreeNode<Int32, Int32>(50, 2);
			var gNode = new ArneTreeNode<Int32, Int32>(18, 3);
			var hNode = new ArneTreeNode<Int32, Int32>(42, 3);
			var iNode = new ArneTreeNode<Int32, Int32>(15, 3);
			var jNode = new ArneTreeNode<Int32, Int32>(45, 3);

			// initialize
			rootNode.Left = aNode;
			rootNode.Right = bNode;

			Assert.AreEqual(null, aNode.Predecessor);
			Assert.AreEqual(null, bNode.Successor);

			aNode.Left = cNode;
			aNode.Right = dNode;
			dNode.Left = gNode;
			gNode.Left = iNode;

			bNode.Left = eNode;
			bNode.Right = fNode;
			eNode.Right = hNode;
			hNode.Right = jNode;

			// perform operation

			Assert.AreEqual(dNode, rootNode.Predecessor);
			Assert.AreEqual(aNode, iNode.Predecessor);

			Assert.AreEqual(eNode, rootNode.Successor);
			Assert.AreEqual(bNode, jNode.Successor);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NodeDecreaseLevel()
		{
			var rootNode = new ArneTreeNode<Int32, Int32>(100, 0);
			var aNode = new ArneTreeNode<Int32, Int32>(90, 4);
			var bNode = new ArneTreeNode<Int32, Int32>(20, 1);
			var cNode = new ArneTreeNode<Int32, Int32>(120, 3);
			var dNode = new ArneTreeNode<Int32, Int32>(10, 5);
			var eNode = new ArneTreeNode<Int32, Int32>(30, 3);

			// initialize
			rootNode.Left = aNode;
			aNode.Left = bNode;
			aNode.Right = cNode;
			bNode.Left = dNode;
			bNode.Right = eNode;

			// perform opeation

			Assert.AreEqual(dNode, dNode.DecraseLevel());
			Assert.AreEqual(eNode, eNode.DecraseLevel());

			aNode.DecraseLevel();

			Assert.AreEqual(2u, aNode.Level);
			Assert.AreEqual(2u, aNode.Right.Level);
		}

		#endregion
	}
}