export interface CreateResurs {
  naziv: string;
  tip: string;
  opis?: string | null;
  cena?: number | null;
  dodeljenKorisniku?: string | null;
}