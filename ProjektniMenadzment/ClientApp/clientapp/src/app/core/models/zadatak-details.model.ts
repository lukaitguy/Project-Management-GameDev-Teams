export interface ZadatakDetails {
  id: string;
  projekatId: string;
  naslov: string;
  opis?: string | null;
  status: string;
  prioritet: string;
  tipZadatka?: string | null;
  rok?: string | null;
  datumKreiranja: string;
  kreiraoKorisnikId: string;
  kreiraoKorisnikIme: string;
  dodeljenKorisnikuId?: string | null;
  dodeljenKorisnikuIme?: string | null;
}