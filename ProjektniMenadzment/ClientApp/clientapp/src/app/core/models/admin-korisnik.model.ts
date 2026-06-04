export interface AdminKorisnik {
  identityUserId: string;
  userName: string;
  email: string;
  roles: string[];
  ime?: string | null;
  prezime?: string | null;
  brojTelefona?: string | null;
  biografija?: string | null;
  projektiCount: number;
  zadaciCount: number;
}

export interface AdminCreateKorisnik {
  userName: string;
  email: string;
  password: string;
  roles: string[];
  ime?: string | null;
  prezime?: string | null;
  brojTelefona?: string | null;
  biografija?: string | null;
}

export interface AdminUpdateKorisnik {
  userName: string;
  email: string;
  roles: string[];
  ime?: string | null;
  prezime?: string | null;
  brojTelefona?: string | null;
  biografija?: string | null;
}
