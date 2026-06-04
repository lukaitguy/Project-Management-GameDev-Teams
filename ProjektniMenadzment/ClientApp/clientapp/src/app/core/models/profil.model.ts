export interface Profil {
  korisnickoIme: string;
  email: string;
  ime: string;
  prezime: string;
  brojTelefona?: string | null;
  biografija?: string | null;
  datumKreiranja: string;
}
