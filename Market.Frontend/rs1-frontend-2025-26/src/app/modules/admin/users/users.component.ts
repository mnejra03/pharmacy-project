import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { UsersApiService, UserProfile } from '../../../api-services/users/users-api.service';

@Component({
  selector: 'app-users', standalone: false,
  template: `<section class="page"><h1>Korisnici</h1>
    <form [formGroup]="filters" (ngSubmit)="search()">
      <mat-form-field><mat-label>Pretraga</mat-label><input matInput formControlName="search"></mat-form-field>
      <mat-form-field><mat-label>Uloga</mat-label><mat-select formControlName="role"><mat-option value="">Sve</mat-option><mat-option value="admin">Administrator</mat-option><mat-option value="pharmacist">Farmaceut</mat-option><mat-option value="customer">Kupac</mat-option></mat-select></mat-form-field>
      <button mat-raised-button type="submit">Filtriraj</button>
    </form>
    <table mat-table [dataSource]="items">
      <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Ime</th><td mat-cell *matCellDef="let u">{{u.firstName}} {{u.lastName}}</td></ng-container>
      <ng-container matColumnDef="email"><th mat-header-cell *matHeaderCellDef>Email</th><td mat-cell *matCellDef="let u">{{u.email}}</td></ng-container>
      <ng-container matColumnDef="role"><th mat-header-cell *matHeaderCellDef>Uloga</th><td mat-cell *matCellDef="let u">{{u.isAdmin?'Administrator':u.isPharmacist?'Farmaceut':'Kupac'}}</td></ng-container>
      <ng-container matColumnDef="actions"><th mat-header-cell *matHeaderCellDef>Akcije</th><td mat-cell *matCellDef="let u"><button mat-button (click)="edit(u)">Uredi</button><button mat-button color="warn" (click)="remove(u.id)">Ukloni</button></td></ng-container>
      <tr mat-header-row *matHeaderRowDef="columns"></tr><tr mat-row *matRowDef="let row;columns:columns"></tr>
    </table><p>Ukupno: {{totalItems}}</p>
    <form *ngIf="selected" [formGroup]="editForm" (ngSubmit)="save()"><h2>Uredi {{selected.email}}</h2>
      <mat-form-field><mat-label>Ime</mat-label><input matInput formControlName="firstName"></mat-form-field><mat-form-field><mat-label>Prezime</mat-label><input matInput formControlName="lastName"></mat-form-field><mat-form-field><mat-label>Telefon</mat-label><input matInput formControlName="phoneNumber"></mat-form-field>
      <mat-checkbox formControlName="isAdmin">Administrator</mat-checkbox><mat-checkbox formControlName="isPharmacist">Farmaceut</mat-checkbox><mat-checkbox formControlName="isCustomer">Kupac</mat-checkbox>
      <button mat-raised-button color="primary" type="submit">Sačuvaj</button><button mat-button type="button" (click)="selected=undefined">Odustani</button>
    </form>
    <button mat-button (click)="previous()" [disabled]="page<=1">Prethodna</button><span> {{page}} / {{totalPages}} </span><button mat-button (click)="next()" [disabled]="page>=totalPages">Sljedeća</button>
  </section>`
})
export class UsersComponent implements OnInit {
  private fb = inject(FormBuilder); private api = inject(UsersApiService);
  filters = this.fb.group({ search: [''], role: [''] }); editForm=this.fb.group({firstName:[''],lastName:[''],phoneNumber:[''],isAdmin:[false],isPharmacist:[false],isCustomer:[false]}); items: UserProfile[] = []; selected?:UserProfile;
  columns = ['name', 'email', 'role','actions']; page = 1; pageSize = 20; totalItems = 0; totalPages = 1;
  ngOnInit(): void { this.load(); }
  search(): void { this.page = 1; this.load(); }
  previous(): void { if (this.page > 1) { this.page--; this.load(); } }
  next(): void { if (this.page < this.totalPages) { this.page++; this.load(); } }
  edit(user:UserProfile):void {this.selected=user;this.editForm.patchValue(user);}
  save():void {if(!this.selected)return;const v=this.editForm.value;if(!v.isAdmin&&!v.isPharmacist&&!v.isCustomer)return;this.api.updateUser(this.selected.id,{firstName:v.firstName??'',lastName:v.lastName??'',phoneNumber:v.phoneNumber??undefined,isAdmin:!!v.isAdmin,isPharmacist:!!v.isPharmacist,isCustomer:!!v.isCustomer}).subscribe(()=>{this.selected=undefined;this.load();});}
  remove(id:number):void {if(confirm('Ukloniti ovog korisnika?'))this.api.deleteUser(id).subscribe(()=>this.load());}
  private load(): void {
    this.api.getUsers({ page: this.page, pageSize: this.pageSize, search: this.filters.value.search || undefined, role: this.filters.value.role || undefined })
      .subscribe(result => { this.items = result.items; this.totalItems = result.totalItems; this.totalPages = Math.max(1, result.totalPages); });
  }
}
