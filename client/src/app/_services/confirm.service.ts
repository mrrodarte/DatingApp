import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef?: BsModalRef<ConfirmDialogComponent>;
  constructor(private modalServices: BsModalService) { }

  confirm(title='Confirmation',message='Are you sure to leave this window?',btnOkText='OK',
    btnCancelText='Cancel'): Observable<boolean> {
      const config = {
        initialState: {
          title,
          message,
          btnOkText,
          btnCancelText
        }
      };
      
      this.bsModalRef = this.modalServices.show(ConfirmDialogComponent, config);
      return this.bsModalRef.onHidden!.pipe(
        map(() => {
          return this.bsModalRef!.content!.result
        })
      )
    }
}
