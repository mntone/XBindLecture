using XBindLecture.DataSets;

namespace XBindLecture.ViewModels
{
	public sealed class PersonViewModel
	{
		public Person OriginalSource { get; }

		public string Name => this.OriginalSource.Name;
		public string Age => this.OriginalSource.Age.ToString();
		public string Sex => this.OriginalSource.Sex.ToString();

		public PersonViewModel(Person originalSource)
		{
			this.OriginalSource = originalSource;
		}
	}
}