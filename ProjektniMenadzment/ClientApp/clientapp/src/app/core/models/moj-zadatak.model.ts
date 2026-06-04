export interface MojZadatak {
  id: string;
  projekatId: string;
  projekatNaziv: string;
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
