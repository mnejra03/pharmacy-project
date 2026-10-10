import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { StoreApiService, Product, ProductCategory, ProductBrand, SaveProduct } from '../../../api-services/store/store-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({selector:'app-admin-products',standalone:false,template:`
<section class="page"><h1>Upravljanje proizvodima</h1>
  <form class="product-form" [formGroup]="form" (ngSubmit)="save()"><h2>{{selected?'Uredi proizvod':'Dodaj proizvod'}}</h2>
    <mat-form-field appearance="outline"><mat-label>Naziv proizvoda</mat-label><input matInput formControlName="name"></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>Opis</mat-label><textarea matInput rows="2" formControlName="description"></textarea></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>Cijena (KM)</mat-label><input matInput type="number" min="0.01" step="0.01" formControlName="price"></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>Stanje</mat-label><input matInput type="number" min="0" formControlName="quantityInStock"></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>URL slike</mat-label><input matInput formControlName="imageUrl"></mat-form-field>
    <label class="image-upload">Ili postavi sliku (JPEG, PNG, WEBP, GIF; do 5 MB)<input type="file" accept="image/jpeg,image/png,image/webp,image/gif" (change)="pickImage($event)"></label>
    <img *ngIf="form.value.imageUrl" class="preview" [src]="form.value.imageUrl" alt="Pregled slike proizvoda" (error)="imageFallback($event)">
    <mat-form-field appearance="outline"><mat-label>Kategorija</mat-label><mat-select formControlName="categoryId"><mat-option *ngFor="let c of categories" [value]="c.id">{{c.name}}</mat-option></mat-select></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>Brend</mat-label><mat-select formControlName="brandId"><mat-option [value]="null">Bez brenda</mat-option><mat-option *ngFor="let b of brands" [value]="b.id">{{b.name}}</mat-option></mat-select></mat-form-field>
    <mat-checkbox formControlName="isDiscounted">Snižena cijena</mat-checkbox>
    <mat-form-field appearance="outline" *ngIf="form.value.isDiscounted"><mat-label>Popust (%)</mat-label><input matInput type="number" min="0.01" max="99.99" formControlName="discountPercentage"></mat-form-field>
    <mat-form-field appearance="outline"><mat-label>Rok trajanja</mat-label><input matInput type="date" formControlName="expiryDate"></mat-form-field>
    <div class="form-actions"><button mat-raised-button color="primary" [disabled]="form.invalid || saving">{{saving?'Spremam...':selected?'Sačuvaj izmjene':'Dodaj proizvod'}}</button><button mat-button type="button" *ngIf="selected" (click)="reset()">Odustani</button></div>
  </form>
  <div class="products-list"><h2>Katalog ({{total}})</h2><div class="table-wrap"><table mat-table [dataSource]="products">
    <ng-container matColumnDef="image"><th mat-header-cell *matHeaderCellDef>Slika</th><td mat-cell *matCellDef="let p"><img class="thumb" [src]="p.imageUrl" [alt]="p.name" (error)="imageFallback($event)"></td></ng-container>
    <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Proizvod</th><td mat-cell *matCellDef="let p">{{p.name}}</td></ng-container>
    <ng-container matColumnDef="category"><th mat-header-cell *matHeaderCellDef>Kategorija</th><td mat-cell *matCellDef="let p">{{p.categoryName}}</td></ng-container>
    <ng-container matColumnDef="stock"><th mat-header-cell *matHeaderCellDef>Stanje</th><td mat-cell *matCellDef="let p">{{p.quantityInStock}}</td></ng-container>
    <ng-container matColumnDef="price"><th mat-header-cell *matHeaderCellDef>Cijena</th><td mat-cell *matCellDef="let p">{{p.currentPrice | number:'1.2-2'}} KM</td></ng-container>
    <ng-container matColumnDef="actions"><th mat-header-cell *matHeaderCellDef>Akcije</th><td mat-cell *matCellDef="let p"><button mat-button (click)="restock(p)">Zaprimanje zalihe</button><button mat-button (click)="edit(p)">Uredi</button><button mat-button color="warn" (click)="remove(p)">Obriši</button></td></ng-container>
    <tr mat-header-row *matHeaderRowDef="columns"></tr><tr mat-row *matRowDef="let row;columns:columns"></tr>
  </table></div><button mat-button (click)="changePage(-1)" [disabled]="page<=1">Prethodna</button> {{page}} / {{totalPages}} <button mat-button (click)="changePage(1)" [disabled]="page>=totalPages">Sljedeća</button></div>
</section>`,styles:[`
  .product-form{display:grid;grid-template-columns:repeat(3,minmax(180px,1fr));align-items:center;gap:0 1rem;padding:1.25rem;border:1px solid #dce6df;border-radius:12px;background:#fff}.product-form h2,.form-actions{grid-column:1/-1}.product-form h2{margin:0 0 1rem}.product-form mat-form-field{width:100%}.form-actions{display:flex;gap:.75rem;padding:0 0 1rem}.image-upload{display:grid;gap:.5rem;padding:.75rem;border:1px dashed #95aa9a;border-radius:8px}.preview{max-width:100%;height:120px;object-fit:contain}.products-list{margin-top:2rem;padding:1rem;border:1px solid #dce6df;border-radius:12px;background:#fff}.products-list h2{margin-top:0}.table-wrap{max-width:100%;overflow-x:auto}.thumb{width:52px;height:52px;object-fit:contain}.products-list table{min-width:780px}@media(max-width:800px){.product-form{grid-template-columns:1fr 1fr}}@media(max-width:540px){.product-form{grid-template-columns:1fr}.product-form h2,.form-actions{grid-column:auto}}
` ]})
export class AdminProductsComponent implements OnInit {
  private api=inject(StoreApiService);private fb=inject(FormBuilder);private toaster=inject(ToasterService);
  categories:ProductCategory[]=[];brands:ProductBrand[]=[];products:Product[]=[];selected?:Product;page=1;pageSize=30;total=0;totalPages=1;saving=false;private imageFile?:File;
  columns=['image','name','category','stock','price','actions'];
  form=this.fb.group({name:['',[Validators.required,Validators.maxLength(200)]],description:['',[Validators.required,Validators.maxLength(4000)]],price:[0,[Validators.required,Validators.min(.01)]],quantityInStock:[0,[Validators.required,Validators.min(0)]],imageUrl:['',[Validators.required,Validators.maxLength(1000)]],categoryId:[null as number|null,[Validators.required]],brandId:[null as number|null],isDiscounted:[false],discountPercentage:[null as number|null],expiryDate:['']});
  ngOnInit(){this.api.getCategories().subscribe(x=>this.categories=x);this.api.getBrands().subscribe(x=>this.brands=x);this.load();}
  load(){this.api.getProducts({page:this.page,pageSize:this.pageSize}).subscribe(x=>{this.products=x.items;this.total=x.totalItems;this.totalPages=Math.max(1,x.totalPages);});}
  changePage(delta:number){this.page+=delta;this.load();}
  pickImage(event:Event){const input=event.target as HTMLInputElement;const file=input.files?.[0];if(!file)return;if(file.size>5*1024*1024){this.toaster.error('Slika može imati najviše 5 MB.');input.value='';return;}this.imageFile=file;this.form.patchValue({imageUrl:''});}
  imageFallback(event:Event){(event.target as HTMLImageElement).src='/images/product-placeholder.svg';}
  edit(p:Product){this.selected=p;this.form.patchValue({name:p.name,description:p.description,price:p.price,quantityInStock:p.quantityInStock,imageUrl:p.imageUrl,categoryId:p.categoryId,brandId:p.brandId??null,isDiscounted:p.isDiscounted,discountPercentage:p.discountPercentage??null,expiryDate:p.expiryDate?.slice(0,10)??''});window.scrollTo({top:0,behavior:'smooth'});}
  reset(){this.selected=undefined;this.imageFile=undefined;this.form.reset({name:'',description:'',price:0,quantityInStock:0,imageUrl:'',categoryId:null,brandId:null,isDiscounted:false,discountPercentage:null,expiryDate:''});}
  save(){if(this.imageFile){this.saving=true;this.api.uploadProductImage(this.imageFile).subscribe({next:r=>{this.imageFile=undefined;this.form.patchValue({imageUrl:r.imageUrl});this.persist();},error:()=>{this.saving=false;this.toaster.error('Slika nije moguće otpremiti.');}});return;}if(this.form.invalid)return;this.saving=true;this.persist();}
  private persist(){const v=this.form.getRawValue();const body:SaveProduct={name:v.name!,description:v.description!,price:Number(v.price),quantityInStock:Number(v.quantityInStock),imageUrl:v.imageUrl!,categoryId:Number(v.categoryId),brandId:v.brandId??undefined,isDiscounted:!!v.isDiscounted,discountPercentage:v.isDiscounted?Number(v.discountPercentage):undefined,expiryDate:v.expiryDate||undefined};const action=this.selected?this.api.updateProduct(this.selected.id,body):this.api.createProduct(body);action.subscribe({next:()=>{this.saving=false;this.reset();this.load();this.toaster.success('Proizvod je sačuvan.');},error:()=>{this.saving=false;this.toaster.error('Proizvod nije moguće sačuvati.');}});}
  restock(p:Product){const raw=prompt(`Koliko komada proizvoda ${p.name} je zaprimljeno?`);if(raw===null)return;const quantity=Number(raw);if(!Number.isInteger(quantity)||quantity<1){this.toaster.error("Unesite cijeli broj veći od nule.");return;}this.api.restockProduct(p.id,quantity).subscribe({next:stock=>{p.quantityInStock=stock;this.toaster.success(`Stanje ažurirano: ${stock} komada.`);},error:()=>this.toaster.error("Zaliha nije ažurirana.")});}
  remove(p:Product){if(confirm(`Obrisati proizvod „${p.name}“?`))this.api.deleteProduct(p.id).subscribe({next:()=>{this.load();this.toaster.success('Proizvod je obrisan.');},error:()=>this.toaster.error('Proizvod nije moguće obrisati.')});}
}
