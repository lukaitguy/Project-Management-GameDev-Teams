export interface ClanIzvestaj {
  imeIPrezime: string;
  uloga?: string | null;
  ukupnoZadataka: number;
  zavrsenihZadataka: number;
  aktivnihZadataka: number;
}

export interface ResursIzvestaj {
  naziv: string;
  tip: string;
  cena?: number | null;
  dodeljenKorisnikuIme?: string | null;
}

export interface ProjekatIzvestaj {
  id: string;
  naziv: string;
  status: string;
  fazaRazvoja?: string | null;
  platforma?: string | null;
  engine?: string | null;
  datumPocetka: string;
  rok?: string | null;
  budzet?: number | null;

  ukupnoZadataka: number;
  zavrsenihZadataka: number;
  uTokuZadataka: number;
  nijeZapocetihZadataka: number;
  pauziranihZadataka: number;
  otkazanihZadataka: number;
  zakasnelihZadataka: number;
  postotakZavrsenosti: number;

  clanovi: ClanIzvestaj[];

  ukupniTroskoviResursa: number;
  preostaliButzet?: number | null;
  resursi: ResursIzvestaj[];
}
