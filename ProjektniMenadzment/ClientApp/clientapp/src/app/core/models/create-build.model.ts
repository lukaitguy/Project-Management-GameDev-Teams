export interface CreateBuild {
  verzija: string;
  nazivBuilda?: string | null;
  tipBuilda?: string | null;
  patchNapomene?: string | null;
  projekatId: string;
}