﻿using FitnessReservatieBL.Exceptions;

namespace FitnessReservatieBL.Domeinen.Eigenschappen
{
    public class ReservatieInfo
    {
        private int _MaxDuurTijdslot = 2;

        public ReservatieInfo(int reservatienummer, int beginuur, int einduur, Toestel toestel) : this(beginuur, einduur, toestel)
        {
            ZetReservatienummer(reservatienummer);
        }

        public ReservatieInfo(int beginuur, int einduur, Toestel toestel)
        {
            ZetBeginuur(beginuur);
            ZetEinduur(einduur);
            ZetToestel(toestel);
        }

        public int Reservatienummer { get; private set; }
        public int Beginuur { get; private set; }
        public int Einduur { get; private set; }
        public Toestel Toestel { get; private set; }

        public void ZetReservatienummer(int reservatienummer)
        {
            if (reservatienummer <= 0) throw new ReservatieInfoException("ReservatieInfo - ZetReservatienummer - 'Mag niet leeg zijn'");
            Reservatienummer = reservatienummer;
        }

        public void ZetBeginuur(int beginuur)
        {
            if (beginuur < 0) throw new ReservatieInfoException("ReservatieInfo - ZetBeginuur - 'Mag niet leeg zijn'");
            if (beginuur > 23) throw new ReservatieInfoException("ReservatieInfo - ZetBeginuur - 'Beginuur kan niet groter zijn dan 24'");
            Beginuur = beginuur;
        }

        public void ZetEinduur(int einduur)
        {
            if (einduur < 0) throw new ReservatieInfoException("ReservatieInfo - ZetEinduur - 'Mag niet leeg zijn'");
            if (einduur > 23) throw new ReservatieInfoException("ReservatieInfo - ZetEinduur - 'Beginuur kan niet groter zijn dan 24'");
            if (einduur < Beginuur) throw new ReservatieInfoException("ReservatieInfo - ZetEinduur - 'Einduur kan niet vroeger zijn dan beginuur'");
            if (einduur > Beginuur+ _MaxDuurTijdslot) throw new ReservatieInfoException("ReservatieInfo - ZetEinduur - 'Maximum duur tijdslot overschreden'");
            Einduur = einduur;
        }

        public void ZetToestel(Toestel toestel)
        {
            if (toestel == null) throw new ReservatieInfoException("ReservatieInfo - ZetToestelType - 'Mag niet leeg zijn'");
            Toestel = toestel;
        }

        public override string ToString()
        {
            return $"{Reservatienummer},{Beginuur},{Einduur},{Toestel}";
        }
    }
}
