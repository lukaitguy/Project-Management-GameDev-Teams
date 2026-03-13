export interface ProjekatDetalji {
  id: string;
  naziv: string;
  opis?: string | null;
  status: string;
  budzet?: number | null;
  datumPocetka: string;
  rok?: string | null;

  verzijaIgre?: string | null;
  engine?: string | null;
  platforma?: string | null;
  fazaRazvoja?: string | null;
  datumPoslednjegBuilda?: string | null;
}