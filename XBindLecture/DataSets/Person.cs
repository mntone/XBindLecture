namespace XBindLecture.DataSets
{
	public sealed class Person
	{
		public string Name { get; }
		public ushort Age { get; }
		public SexType Sex { get; }

		public Person(string name, ushort age, SexType sex)
		{
			this.Name = name;
			this.Age = age;
			this.Sex = sex;
		}
	}
}