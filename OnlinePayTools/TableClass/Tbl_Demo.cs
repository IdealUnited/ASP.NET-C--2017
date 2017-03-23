/*----------------------------------------------------------------------------------------------------------------
Version:	CSAdonetCodeWriter 1.00
   Time:	2012/12/4 19:47:03
    SQL:	SELECT * FROM Tbl_Demo
----------------------------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
	[DBTableAttribute("Tbl_Demo", "Guid")]
	public class Tbl_Demo : IDBOperability
    {
        #region AutoIncrement
        private static readonly DBTableAttribute m_AutoIncrement = new DBTableAttribute("Tbl_Demo", "Guid", "Id");

        public static DBTableAttribute AutoIncrement
        {
            get { return Tbl_Demo.m_AutoIncrement; }
        } 
        #endregion

		#region Field and Property
		
		/// <summary>
		/// 0 ID Int32
		/// </summary>
		protected Int32 m_ID;

		/// <summary>
		/// 0 ID Int32
		/// </summary>
		public Int32 ID
		{
			get
			{
				return m_ID;
			}
			set
			{
				m_ID = value;
			}
		}

		/// <summary>
		/// 1 Guid String
		/// </summary>
		protected String m_Guid;

		/// <summary>
		/// 1 Guid String
		/// </summary>
		public String Guid
		{
			get
			{
				return m_Guid;
			}
			set
			{
				m_Guid = value;
			}
		}

		/// <summary>
		/// 2 Name String
		/// </summary>
		protected String m_Name;

		/// <summary>
		/// 2 Name String
		/// </summary>
		public String Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		/// <summary>
		/// 3 Birthday DateTime
		/// </summary>
		protected DateTime? m_Birthday;

		/// <summary>
		/// 3 Birthday DateTime
		/// </summary>
		public DateTime? Birthday
		{
			get
			{
				return m_Birthday;
			}
			set
			{
				m_Birthday = value;
			}
		}

		/// <summary>
		/// 4 Photo Byte[]
		/// </summary>
		protected Byte[] m_Photo;

		/// <summary>
		/// 4 Photo Byte[]
		/// </summary>
		public Byte[] Photo
		{
			get
			{
				return m_Photo;
			}
			set
			{
				m_Photo = value;
			}
		}

		
		#endregion

		#region IDBOperability

		/// <summary>
		/// 1 Guid String
		/// </summary>
		Object IDBOperability.PrimaryKey
		{
			get
			{
				return this.Guid;
			}
			set
			{
				this.Guid = (String)value;
			}
		}

		void IDBOperability.SetRecordData(IDBRecord dbRecord, Object oFlag)
		{
            IDBRecord x = dbRecord;

			//x["ID"] = ID; // 0 Int32
			x["Guid"] = Guid; // 1 String
			x["Name"] = Name; // 2 String
			x["Birthday"] = Birthday; // 3 DateTime
			x["Photo"] = Photo; // 4 Byte[]
			/*
			//x[0] = ID; // ID Int32
			x[1] = Guid; // Guid String
			x[2] = Name; // Name String
			x[3] = Birthday; // Birthday DateTime
			x[4] = Photo; // Photo Byte[]
			*/
		}

		void IDBOperability.GetRecordData(IDBRecord dbRecord, Object oFlag)
		{
            IDBRecord x = dbRecord;

			ID = DBValue.Convert(x["ID"]); // 0 Int32
			Guid = DBValue.Convert(x["Guid"]); // 1 String
			Name = DBValue.Convert(x["Name"]); // 2 String
			Birthday = DBValue.Convert(x["Birthday"]); // 3 DateTime
			Photo = DBValue.Convert(x["Photo"]); // 4 Byte[]
			/*
			ID = DBValue.Convert(x[0]) // ID Int32
			Guid = DBValue.Convert(x[1]) // Guid String
			Name = DBValue.Convert(x[2]) // Name String
			Birthday = DBValue.Convert(x[3]) // Birthday DateTime
			Photo = DBValue.Convert(x[4]) // Photo Byte[]
			*/
		}

		#endregion
	}
}
