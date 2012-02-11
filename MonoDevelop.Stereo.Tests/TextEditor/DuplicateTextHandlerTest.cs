using MonoDevelop.Components.Commands;
using MonoDevelop.Stereo.TextEditor;
using NUnit.Framework;
using Rhino.Mocks;

namespace MonoDevelop.Stereo.DuplicateTextHandlerTest
{	
	public class DuplicateTextHandlerToTest : DuplicateTextHandler{
		public DuplicateTextHandlerToTest (ITextDuplicationContext ctx) : base(ctx) {}
		public void TestUpdate(CommandInfo info){base.Update (info);}
		public void TestRun(){base.Run ();}
	}
	
	[TestFixture]
	public class Update
	{
		ITextDuplicationContext ctx = MockRepository.GenerateStub<ITextDuplicationContext>();
		DuplicateTextHandlerToTest subject;
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new DuplicateTextHandlerToTest(ctx);
		}
		
		[SetUp]
		public void BeforeEach ()
		{
			ctx.BackToRecord(BackToRecordOptions.All);
			ctx.Replay();
		}
		
		[Test]
		public void Enables_info_when_active_document_and_editor_exist(){
			ctx.Stub(p=>p.ActiveDocumentAndEditorExist()).Return(true);
			
			CommandInfo commandInfo = new CommandInfo{Enabled=false};
			subject.TestUpdate(commandInfo);
			
			Assert.IsTrue(commandInfo.Enabled);
		}
		
		[Test]
		public void Enables_info_when_active_document_and_editor_dont_exist(){
			ctx.Stub(p=>p.ActiveDocumentAndEditorExist()).Return(false);
			
			CommandInfo commandInfo = new CommandInfo{Enabled=false};
			subject.TestUpdate(commandInfo);
			
			Assert.IsFalse(commandInfo.Enabled);
		}
	}
	
	[TestFixture]
	public class Run
	{
		ITextDuplicationContext ctx = MockRepository.GenerateStub<ITextDuplicationContext>();
		DuplicateText text = new EmptyDuplicateText();
		
		DuplicateTextHandlerToTest subject;
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new DuplicateTextHandlerToTest(ctx);
		}
		
		[Test]
		public void Appends_duplicated_text(){
			ctx.Stub(p=>p.GetTextToDuplicate()).Return(text);
			
			subject.TestRun();
			
			ctx.AssertWasCalled(p=>p.AppendDuplicatedText(text));
		}
	}
}