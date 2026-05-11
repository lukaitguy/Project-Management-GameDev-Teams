export interface CreateZadatak {
  naslov: string;
  opis?: string | null;
  status: string;
  prioritet: string;
  tipZadatka?: string | null;
  rok?: string | null;
  dodeljenKorisnikuId?: string | null;
}