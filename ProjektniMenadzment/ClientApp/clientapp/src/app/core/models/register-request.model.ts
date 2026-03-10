export interface RegisterRequest {
  ime: string;
  prezime: string;
  korisnickoIme: string;
  email: string;
  brojTelefona?: string | null;
  lozinka: string;
}