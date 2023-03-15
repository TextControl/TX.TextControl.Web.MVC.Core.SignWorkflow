namespace tx_viewer_sign.Models {
	public class FormData {
		public Patient patient { get; set; }
	}

	public class Patient {
		public string Name { get; set; }
		public string Firstname { get; set; }
		public DateTime DOB { get; set; }
		public string Gender { get; set; }
		public string Street { get; set; }
		public string State { get; set; }
		public string ZIP { get; set; }
		public string City { get; set; }
	}

	public class SignData {
		public FormData formData { get; set; }	
		public byte[] document {  get; set; }
	}
}

