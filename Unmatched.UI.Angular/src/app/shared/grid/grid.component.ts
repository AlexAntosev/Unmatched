import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { GridColumn } from './grid-column';
import { GridColumnType } from './grid-column-type';

@Component({
  selector: 'app-grid',
  standalone: false,
  templateUrl: './grid.component.html',
  styleUrl: './grid.component.css',
})
export class GridComponent implements OnInit, AfterViewInit {
  @Input() public itemsSbj$ = new BehaviorSubject<any[]>([]);
  @Input() public columns: GridColumn<any>[] = [];
  @Input() public defaultSort: string = 'name';

  @Output() public onRowClicked = new EventEmitter<any>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<any>;

  get columnsIds(): string[] {
    return this.columns.map((c) => c.id);
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => {
      const items = this.sortItems(this.itemsSbj$.getValue());
      this.setItems(items);
    });
  }

  setItems(items: any[]) {
    items.map((item) => {
      item.place = items.indexOf(item) + 1;
    });
    this.itemsSbj$.next(items);
  }

  sortItems(items: any[]) {
    return items.sort((a, b) => {
      const isAsc = this.sort.direction === 'asc';
      const column = this.columns.find((c) => c.id === this.sort.active);
      if (column) {
        return this.compare(
          column.valueGetter(a),
          column.valueGetter(b),
          isAsc
        );
      }
      return 0;
    });
  }

  compare(
    a: number | string | boolean,
    b: number | string | boolean,
    isAsc: boolean
  ) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  rowClicked(item: any) {
    this.onRowClicked.emit(item);
  }

  isTextColumn(type: GridColumnType) {
    return type === GridColumnType.Text;
  }

  isImageColumn(type: GridColumnType) {
    return type === GridColumnType.Image;
  }
}
