using System;
using System.Drawing;
using CommandSerializer;
using CommandSerializer.Exceptions;
using ConsoleArgsTests.TestClasses;
using NUnit.Framework;

namespace ConsoleArgsTests
{
    public class CommandSerializerTest
    {
        [Test]
        public void TestAlias()
        {
            var result = CommandSerializer<AliasTest>.Parse(new[] {"-s"});
            Assert.IsTrue(result.IsSet);
        }

        [Test]
        public void TestAliases()
        {
            var result = CommandSerializer<AliasesTest>.Parse(new[] { "-sS" });
            Assert.IsTrue(result.Set1);
            Assert.IsTrue(result.Set2);
            Assert.IsFalse(result.Set3);
        }

        [Test]
        public void TestAction()
        {
            var result = CommandSerializer<ActionTest>.Parse(new[] { "--set-me" });
            Assert.IsTrue(result.IsSet);
        }

        [Test]
        public void TestActions()
        {
            var result = CommandSerializer<ActionsTest>.Parse(new[] { "--set1", "--set2" });
            Assert.IsTrue(result.Set1);
            Assert.IsTrue(result.Set2);
            Assert.IsFalse(result.Set3);
        }

        [Test]
        public void TestActionsWithInputStrings()
        {
            var result = CommandSerializer<ActionsTestWithInputString>.Parse(new[] { "--name", "foo", "--password", "bar" });
            Assert.AreEqual("foo", result.Name);
            Assert.AreEqual("bar", result.Password);
        }

        [Test]
        public void TestRequired()
        {
            var result = CommandSerializer<RequiredTest>.Parse(new[] { "--name", "foo", "-v" });
            Assert.AreEqual("foo", result.Name);
            Assert.IsTrue(result.IsValid);

            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<RequiredTest>.Parse(new string[] { });
            });

            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<RequiredTest>.Parse(new[] {"-v"});
            });

            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<RequiredTest>.Parse(new[] {"--name", "foo"});
            });
        }

        [Test]
        public void TestArray()
        {
            var result = CommandSerializer<ArrayTest>.Parse(new[] { "--array", "1", "2", "100" });
            Assert.AreEqual(3, result.Numbers.Length);
            Assert.AreEqual(1, result.Numbers[0]);
            Assert.AreEqual(2, result.Numbers[1]);
            Assert.AreEqual(100, result.Numbers[2]);
        }

        [Test]
        public void TestList()
        {
            var result = CommandSerializer<ListTest>.Parse(new[] { "-c", "red", "green", "blue" });
            Assert.AreEqual(3, result.Color.Count);
            Assert.AreEqual("red", result.Color[0]);
            Assert.AreEqual("green", result.Color[1]);
            Assert.AreEqual("blue", result.Color[2]);
        }

        [Test]
        public void TestEnum()
        {
            var result = CommandSerializer<EnumTest>.Parse(new[] { "--color", "blue" });
            Assert.AreEqual(EnumTest.Color.Blue, result.Col);
        }

        [Test]
        public void TestDate()
        {
            var date = "2018-08-18T07:22:16.0000000Z";
            var result = CommandSerializer <DateTest>.Parse(new[] { "--date", date });
            Assert.AreEqual(DateTime.Parse(date), result.Date);
        }

        [Test]
        public void TestNonExistingAlias()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<AliasTest>.Parse(new[] { "-a" });
            });
        }

        [Test]
        public void TestNonExistingAction()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<AliasTest>.Parse(new[] { "--aaa" });
            });
        }

        [Test]
        public void TestInvalidMultipleAlias()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<AliasesTest>.Parse(new[] { "-stv", "test" });
            });
        }

        [Test]
        public void TestMissingInput()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<ActionsTestWithInputString>.Parse(new[] {"--name", "--password", "bar"});
            });
        }

        [Test]
        public void TestActionNameInsensitive()
        {
            var result = CommandSerializer<ActionTest>.Parse(new[] { "--SET-ME" });
            Assert.IsTrue(result.IsSet);
        }

        [Test]
        public void TestBasicClass()
        {
            var result = CommandSerializer<BasicClassTest>.Parse(new[] { "--isCar", "--wheelCount", "4", "--Colors", "red", "black" });
            Assert.IsTrue(result.IsCar);
            Assert.AreEqual(4, result.WheelCount);
            Assert.AreEqual(2, result.Colors.Count);
            Assert.AreEqual("red", result.Colors[0]);
            Assert.AreEqual("black", result.Colors[1]);
        }

        [Test]
        public void TestPositionalArgs()
        {
            var result = CommandSerializer<PositionalTest>.Parse(new[] { "blue", "-a", "foo", "bar", "2000" });
            Assert.IsTrue(result.IsAutomatic);
            Assert.AreEqual("blue", result.Color);
            Assert.AreEqual(3, result.Features.Count);
            Assert.AreEqual("foo", result.Features[0]);
            Assert.AreEqual("bar", result.Features[1]);
            Assert.AreEqual("2000", result.Features[2]);
        }

        [Test]
        public void TestRequiredPositionalArgs()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<PositionalRequiredTest>.Parse(new string[] { });
            });
        }

        [Test]
        public void TestActionsWithMissingParameter()
        {
            Assert.Throws<CommandArgumentException>(delegate
            {
                CommandSerializer<ActionsTestWithInputString>.Parse(new[] { "--name", "foo", "--password" });
            });
        }

        [Test]
        public void TestFileAndDirectory()
        {
            var result = CommandSerializer<FileAndDirectoryTest>.Parse(new[] { "--file", "toto.txt", "--directory", "foo/bar" });
            Assert.IsFalse(result.File.Exists);
            Assert.IsFalse(result.Directory.Exists);
        }

        [Test]
        public void TestConverter()
        {
            var result = CommandSerializer<ConverterTest>.Parse(new[] { "-c", "blue" });
            Assert.AreEqual(result.Color, Color.Blue);
        }

        [Test]
        public void TestHelp()
        {
            var helpText = CommandSerializer<HelpTest>.GetHelp();
            TestContext.Out.WriteLine(helpText);
            System.Diagnostics.Debug.WriteLine(helpText);
            Assert.IsNotEmpty(helpText);
        }

        //
        // TODO: test with same flags with 2 properties => should work I suppose since it will be the same??? Need to think about it
    }
}