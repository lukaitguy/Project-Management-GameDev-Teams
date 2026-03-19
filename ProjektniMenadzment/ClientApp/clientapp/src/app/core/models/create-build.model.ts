export interface CreateBuild {
  verzija: string;
  nazivBuilda?: string | null;
  tipBuilda?: string | null;
  patchNapomene?: string | null;
  datumBuilda?: string | null;
  projekatId: string;
}