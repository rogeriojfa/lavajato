using System;

namespace GestaoEventos.Core.Aggregates.AuthAgg.Dtos
{
    public class UserForRegisterDto
    {
        private int? _idUserWhoChange;
        public string Name { get; set; }
        public int Cell { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public DateTime ChangeDate { get; }

        public int? IdUserWhoChange => _idUserWhoChange;

        public int IdProfile { get; set; }

        public UserForRegisterDto()
        {
            ChangeDate = DateTime.Now;
        }

        public void SetIdUserWhoChange(int idUserWhoChange)
        {
            _idUserWhoChange = idUserWhoChange;
        }
    }
}
