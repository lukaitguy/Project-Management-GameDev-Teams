export interface CreateProjekat {
  naziv: string;
  opis?: string | null;
  status: string;
  budzet?: number | null;
  datumPocetka: string;
  rok?: string | null;
  zanrId?: string | null;

  verzijaIgre?: string | null;
  engine?: string | null;
  platforma?: string | null;
  fazaRazvoja?: string | null;
}