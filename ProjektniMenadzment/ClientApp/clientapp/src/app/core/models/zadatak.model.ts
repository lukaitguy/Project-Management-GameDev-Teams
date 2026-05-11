export interface Zadatak {
  id: string;
  projekatId: string;
  naslov: string;
  opis?: string | null;
  status: string;
  prioritet: string;
  tipZadatka?: string | null;
  rok?: string | null;
  datumKreiranja: string;
  dodeljenKorisnikuId?: string | null;
  dodeljenKorisnikuIme?: string | null;
}