import { Clock } from 'lucide-react';
import { BookmarkType, Content } from '../Models/types';

type GameResult = {
  outcome: 'Win' | 'Loss' | 'Draw' | null;
  teamScore: number;
  enemyScore: number;
  detail: string | null;
};
const OUTCOME = {
  Win: ['Victory', 'text-success bg-success/10'],
  Loss: ['Defeat', 'text-error bg-error/10'],
  Draw: ['Draw', 'text-gray-300 bg-base-100'],
};
const KDA = [BookmarkType.Kill, BookmarkType.Death, BookmarkType.Assist];

// Fork: icon, score and duration cells of a session banner row (laid out by .fayo-rows in theme.css).
export default function SessionStats({
  content,
  icon,
}: {
  content: Content & { result?: GameResult };
  icon?: string | null;
}) {
  const r = content.result;
  const kda = KDA.map((t) => content.bookmarks?.filter((b) => b.type === t).length ?? 0);
  return (
    <div className="contents">
      <div className="fayo-icon">{icon && <img src={icon} alt="" />}</div>
      <div className="fayo-score">
        {r ? (
          <>
            <span className="text-2xl">
              <span className="text-success">{r.teamScore}</span> –{' '}
              <span className="text-error">{r.enemyScore}</span>
            </span>
            {r.outcome && <span className={OUTCOME[r.outcome][1]}>{OUTCOME[r.outcome][0]}</span>}
            {r.detail && <small>{r.detail}</small>}
          </>
        ) : kda.some(Boolean) ? (
          <>
            <span className="text-lg">{kda.join(' / ')}</span>
            <small>K / D / A</small>
          </>
        ) : (
          '—'
        )}
      </div>
      <div className="fayo-dur">
        <Clock size={14} />
        {content.duration
          .split('.')[0]
          .replace(/^00:/, '')
          .replace(/^0(\d)/, '$1')}
      </div>
    </div>
  );
}
