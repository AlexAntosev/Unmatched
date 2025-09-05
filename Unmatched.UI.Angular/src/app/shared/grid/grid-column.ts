import { GridColumnType } from './grid-column-type';

export class GridColumn<T> {
  id: string = '';
  header: string = '';
  type: GridColumnType = GridColumnType.Text;
  valueGetter: (item: T) => any = () => '';
}
