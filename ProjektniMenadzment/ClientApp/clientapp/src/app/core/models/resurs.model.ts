export interface Resurs {
  id: string;
  naziv: string;
  tip: string;
  opis?: string | null;
  cena?: number | null;
  dodeljenKorisniku?: string | null;
  dodeljenKorisnikuIme?: string | null;
  datumKreiranja: string;
}