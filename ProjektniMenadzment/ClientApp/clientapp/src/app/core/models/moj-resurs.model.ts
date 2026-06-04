export interface MojResurs {
  id: string;
  projekatId: string;
  projekatNaziv: string;
  naziv: string;
  tip: string;
  opis?: string | null;
  cena?: number | null;
  dodeljenKorisniku?: string | null;
  dodeljenKorisnikuIme?: string | null;
  datumKreiranja: string;
}
