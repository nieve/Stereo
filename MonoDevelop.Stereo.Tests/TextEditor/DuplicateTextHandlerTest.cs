using MonoDevelop.Components.Commands;
using MonoDevelop.Stereo.TextEditor;
using NUnit.Framework;
using Rhino.Mocks;

namespace MonoDevelop.Stereo.DuplicateTextHandlerTest
{	
	public class DuplicateTextHandlerToTest : DuplicateTextHandler{
		public DuplicateTextHandlerToTest (IParseDocument parser) : base(parser) {}
		public void TestUpdate(CommandInfo info){base.Update (info);}
		public void TestRun(){base.Run ();}
	}
	
	[TestFixture]
	public class Update
	{
		IParseDocument docParser = MockRepository.GenerateStub<IParseDocument>();
		DuplicateTextHandlerToTest subject;
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new DuplicateTextHandlerToTest(docParser);
		}
		
		[SetUp]
		public void BeforeEach ()
		{
			docParser.BackToRecord(BackToRecordOptions.All);
			docParser.Replay();
		}
		
		[Test]
		public void Enables_info_when_active_document_and_editor_exist(){
			docParser.Stub(p=>p.ActiveDocumentAndEditorExist()).Return(true);
			
			CommandInfo commandInfo = new CommandInfo{Enabled=false};
			subject.TestUpdate(commandInfo);
			
			Assert.IsTrue(commandInfo.Enabled);
		}
		
		[Test]
		public void Enables_info_when_active_document_and_editor_dont_exist(){
			docParser.Stub(p=>p.ActiveDocumentAndEditorExist()).Return(false);
			
			CommandInfo commandInfo = new CommandInfo{Enabled=false};
			subject.TestUpdate(commandInfo);
			
			Assert.IsFalse(commandInfo.Enabled);
		}
	}
	
	[TestFixture]
	public class Run
	{
		IParseDocument docParser = MockRepository.GenerateStub<IParseDocument>();
		DuplicateText text = new EmptyDuplicateText();
		
		DuplicateTextHandlerToTest subject;
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new DuplicateTextHandlerToTest(docParser);
		}
		
		[Test]
		public void Appends_duplicated_text(){
			docParser.Stub(p=>p.GetTextToDuplicate()).Return(text);
			
			subject.TestRun();
			
			docParser.AssertWasCalled(p=>p.AppendDuplicatedText(text));
		}
	}
}